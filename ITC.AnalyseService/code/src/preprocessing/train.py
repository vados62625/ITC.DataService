import pandas as pd
import numpy as np
from sklearn.ensemble import RandomForestRegressor
from sklearn.model_selection import train_test_split
from sklearn.multioutput import MultiOutputRegressor
from sklearn.metrics import mean_squared_error
import joblib
import matplotlib.pyplot as plt

# Загрузка размеченных данных
data = pd.read_csv("labeled_data/all_labeled.csv")

# Разделение на признаки и метки
X = data[["mean", "std", "skew", "kurtosis", "peak", "crest_factor", "kurtosis_env"]]
y = data[["outer_race", "inner_race", "rolling_element", "cage", "unbalance", "misalignment"]]

# Разделение данных
X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2, random_state=42)

# Обучение модели
model = MultiOutputRegressor(
    RandomForestRegressor(n_estimators=200, max_depth=25, min_samples_split=5, random_state=42, n_jobs=-1, verbose=1)
)
model.fit(X_train, y_train)

# Оценка модели
y_pred = model.predict(X_test)

# Вывод RMSE для каждого дефекта
defect_names = y.columns
rmse_scores = {}
for i, defect in enumerate(defect_names):
    rmse = np.sqrt(mean_squared_error(y_test.iloc[:, i], y_pred[:, i]))
    rmse_scores[defect] = rmse
    print(f"{defect}: RMSE = {rmse:.4f}")

# Визуализация важности признаков
plt.figure(figsize=(12, 8))
for i, defect in enumerate(defect_names):
    importances = model.estimators_[i].feature_importances_
    plt.subplot(2, 3, i + 1)
    plt.barh(X.columns, importances)
    plt.title(f"Feature Importances for {defect}")
plt.tight_layout()
plt.savefig("feature_importances.png")

# Сохранение модели
joblib.dump(model, "defect_model.pkl")
print("Model saved as defect_model.pkl")
