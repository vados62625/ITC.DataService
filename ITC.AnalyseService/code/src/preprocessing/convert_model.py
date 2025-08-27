import os
import joblib

from skl2onnx import convert_sklearn
from skl2onnx.common.data_types import FloatTensorType
from src.common.config import settings


def convert_model_to_onnx():
    """Функция для конвертации модели в ONNX формат"""
    
    # Загрузка оригинальной модели
    model = joblib.load(settings.MODEL_PATH)
    
    # Создаем директорию для ONNX моделей, если её нет
    os.makedirs(settings.ONNX_MODEL_DIR, exist_ok=True)
    
    n_features = 7 
    initial_type = [('input', FloatTensorType([None, n_features]))]
    
    # Конвертируем каждый estimator отдельно
    for i, estimator in enumerate(model.estimators_):
        try:
            onnx_estimator = convert_sklearn(
                estimator,
                initial_types=initial_type,
                target_opset=13
            )
            
            # Сохраняем модель
            model_path = f"{settings.ONNX_MODEL_DIR}/model_estimator_{i}.onnx"
            with open(model_path, "wb") as f:
                f.write(onnx_estimator.SerializeToString())
                
            print(f"Estimator {i} ({model.estimators_[i].__class__.__name__}) конвертирован и сохранен в {model_path}")
            
        except Exception as e:
            print(f"Ошибка при конвертации estimator {i}: {e}")


if __name__ == "__main__":
    convert_model_to_onnx()
