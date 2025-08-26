import numpy as np
import pandas as pd
import joblib
from loguru import logger
from scipy import signal, stats

from src.common.config import settings
import warnings

import os
import sys
from contextlib import contextmanager


@contextmanager
def suppress_output():
    with open(os.devnull, "w") as devnull:
        old_stdout = sys.stdout
        old_stderr = sys.stderr
        sys.stdout = devnull
        sys.stderr = devnull
        try:
            yield
        finally:
            sys.stdout = old_stdout
            sys.stderr = old_stderr


warnings.filterwarnings("ignore")


class ProcessData:
    def __init__(self) -> None:
        self.model = joblib.load(settings.MODEL_PATH)

    def predict(self, data: list[list[float]]) -> dict[str, float]:
        defects = [
            "Дефект наружного кольца",
            "Дефект внутреннего кольца",
            "Дефект тел качения",
            "Дефект сепаратора",
            "Дисбаланс",
            "Расцентровка",
        ]
        logger.info(len(data))

        df = pd.DataFrame(data, columns=["R", "S", "T"])
        df = df.dropna(axis=1, how="all")

        # Обработка сегментов
        n_segments = len(df) // settings.SEGMENT_LENGTH
        predictions = []

        for i in range(n_segments):
            start_idx = i * settings.SEGMENT_LENGTH
            end_idx = (i + 1) * settings.SEGMENT_LENGTH
            segment = df.iloc[start_idx:end_idx].values

            # Извлечение признаков
            features = self.extract_features(segment)
            if features.shape[0] == 0:
                continue

            # Предсказание
            with suppress_output():
                pred = self.model.predict(features)
            predictions.append(np.mean(pred, axis=0))

        if len(predictions) == 0:
            logger.error("No valid segments found")
            results = dict()
            for name in defects:
                results[name] = 0
                print(f"{name}: {0}")
            return results

        # Усреднение результатов
        avg_pred = np.mean(predictions, axis=0)

        print("Степени развития дефектов:")
        results = dict()
        for name, value in zip(defects, avg_pred):
            results[name] = value
            print(f"{name}: {value:.4f}")
        return results

    def extract_features(self, segment):
        """Извлечение признаков из сегмента данных"""
        features = []

        # Для каждой фазы
        for phase in range(segment.shape[1]):
            x = segment[:, phase]

            # Пропустить NaN
            x = x[~np.isnan(x)]
            if len(x) < settings.SEGMENT_LENGTH:
                continue

            # Вычисление RMS
            rms = np.sqrt(np.mean(x**2))
            if rms < 1e-5:
                continue

            # Временные признаки
            mean_val = np.mean(x)
            std_val = np.std(x)
            skew_val = stats.skew(x)
            kurt_val = stats.kurtosis(x)
            peak_val = np.max(np.abs(x))
            crest_factor = peak_val / rms

            # Куртозис огибающей
            analytic_signal = signal.hilbert(x)
            envelope = np.abs(analytic_signal)
            kurt_env = stats.kurtosis(envelope)

            features.append([mean_val, std_val, skew_val, kurt_val, peak_val, crest_factor, kurt_env])

        return np.array(features)
