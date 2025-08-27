import numpy as np
import pandas as pd
from loguru import logger
from scipy import signal, stats
import onnxruntime as rt
import warnings

from src.common.config import settings

warnings.filterwarnings("ignore")


class ProcessData:
    def __init__(self) -> None:
        self.sessions = []
        self.input_names = []
        
        for i in range(6):  
            model_path = f"{settings.ONNX_MODEL_DIR}/model_estimator_{i}.onnx"
            session = rt.InferenceSession(model_path)
            self.sessions.append(session)
            self.input_names.append(session.get_inputs()[0].name)
        
        self.defects = [
            "Дефект наружного кольца",
            "Дефект внутреннего кольца",
            "Дефект тел качения",
            "Дефект сепаратора",
            "Дисбаланс",
            "Расцентровка",
        ]

    def predict(self, data: list[list[float]]) -> dict[str, float]:
        logger.info(f"Received data length: {len(data)}")
        
        df = pd.DataFrame(data, columns=["R", "S", "T"])
        df = df.dropna(axis=1, how='all')
        
        n_segments = len(df) // settings.SEGMENT_LENGTH
        if n_segments == 0:
            logger.error("No segments available after preprocessing")
            return {defect: 1.0 for defect in self.defects}

        # Precompute segments
        segments = np.array_split(df.values[:n_segments * settings.SEGMENT_LENGTH], n_segments)
        
        # Extract features for all segments
        all_features = []
        for segment in segments:
            features = self.extract_features(segment)
            if features.size > 0:
                all_features.append(features)
        
        if not all_features:
            logger.error("No valid features extracted")
            return {defect: 1.0 for defect in self.defects}

        # Для каждого сегмента делаем предсказания для всех estimators
        all_predictions = []
        
        for features in all_features:
            segment_predictions = []
            
            # Для каждого estimator (дефекта) делаем предсказание
            for i, session in enumerate(self.sessions):
                # Подготавливаем входные данные
                input_data = features.astype(np.float32)
                
                # Делаем предсказание
                ort_inputs = {self.input_names[i]: input_data}
                ort_pred = session.run(None, ort_inputs)[0]
                
                # Усредняем предсказания для всех фаз в сегменте
                segment_predictions.append(np.mean(ort_pred))
            
            all_predictions.append(segment_predictions)
        
        # Усредняем предсказания по всем сегментам
        avg_pred = np.mean(all_predictions, axis=0)
        
        logger.info("Prediction completed successfully")
        return {defect: float(score) for defect, score in zip(self.defects, avg_pred)}

    def extract_features(self, segment: np.ndarray) -> np.ndarray:
        """Векторизованное извлечение признаков из сегмента данных"""
        # Удаляем строки с NaN значениями
        segment = segment[~np.isnan(segment).any(axis=1)]
        if segment.size == 0 or len(segment) < settings.SEGMENT_LENGTH:
            return np.array([])

        # Вычисляем RMS для всех фаз одновременно
        rms = np.sqrt(np.mean(segment**2, axis=0))
        valid_phases = rms >= 1e-5
        if not np.any(valid_phases):
            return np.array([])

        # Применяем valid_phases маску
        segment_valid = segment[:, valid_phases]
        rms_valid = rms[valid_phases]

        # Вычисляем все статистики одновременно
        mean_val = np.mean(segment_valid, axis=0)
        std_val = np.std(segment_valid, axis=0)
        skew_val = stats.skew(segment_valid, axis=0)
        kurt_val = stats.kurtosis(segment_valid, axis=0)
        peak_val = np.max(np.abs(segment_valid), axis=0)
        crest_factor = np.divide(peak_val, rms_valid, where=rms_valid != 0)

        # Вычисляем огибающую через преобразование Гильберта
        analytic_signal = signal.hilbert(segment_valid, axis=0)
        envelope = np.abs(analytic_signal)
        kurt_env = stats.kurtosis(envelope, axis=0)

        # Комбинируем все признаки
        features = np.vstack([
            mean_val, std_val, skew_val, kurt_val, peak_val, crest_factor, kurt_env
        ]).T

        return features
