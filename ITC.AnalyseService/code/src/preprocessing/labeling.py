import os
import numpy as np
import pandas as pd
from scipy import signal, stats
import matplotlib.pyplot as plt
from loguru import logger
import warnings

# Игнорируем предупреждения
warnings.filterwarnings("ignore")

# Параметры установки
FS = 25600  # Частота дискретизации (Гц)
SEGMENT_LENGTH = 65536  # Длина сегмента для лучшего частотного разрешения
MOTOR_RPM = 1770
OUTPUT_RPM = 3010
F_MOTOR = MOTOR_RPM / 60.0  # Частота вращения ротора (Гц)

# Параметры подшипника NSK6205DDU (проверенные значения)
BEARING_PARAMS = {
    "n_balls": 9,
    "d_ball": 7.94e-3,
    "pitch_dia": 39.04e-3,
    "contact_angle": np.deg2rad(0),  # Угол в радианах
}


def compute_defect_frequencies():
    """Вычисляет характерные частоты дефектов с проверенными формулами"""
    # Характерные частоты для подшипника двигателя
    n = BEARING_PARAMS["n_balls"]
    d = BEARING_PARAMS["d_ball"]
    D = BEARING_PARAMS["pitch_dia"]
    phi = BEARING_PARAMS["contact_angle"]

    # Правильные формулы для характерных частот
    bpfo = n * F_MOTOR / 2 * (1 - (d / D) * np.cos(phi))
    bpfi = n * F_MOTOR / 2 * (1 + (d / D) * np.cos(phi))
    bsf = (D * F_MOTOR) / (2 * d) * (1 - (d / D) ** 2 * np.cos(phi) ** 2)
    ftf = F_MOTOR / 2 * (1 - (d / D) * np.cos(phi))

    return {
        "outer_race": bpfo,
        "inner_race": bpfi,
        "rolling_element": bsf,
        "cage": ftf,
        "unbalance": F_MOTOR,  # Частота дисбаланса (1x)
        "misalignment": 2 * F_MOTOR,  # Частота расцентровки (2x)
    }


def analyze_envelope(x, fs, defect_freqs):
    """Анализ огибающей сигнала для выявления дефектов подшипников"""
    # Вычисление огибающей через преобразование Гильберта
    analytic_signal = signal.hilbert(x)
    envelope = np.abs(analytic_signal)

    # Удаление постоянной составляющей
    envelope = envelope - np.mean(envelope)

    # Спектральный анализ огибающей
    nperseg = min(8192, len(envelope))
    f, Pxx_env = signal.welch(envelope, fs, nperseg=nperseg, scaling="spectrum")

    defect_levels = {}
    for defect, base_freq in defect_freqs.items():
        if defect in ["unbalance", "misalignment"]:
            continue  # Эти дефекты анализируем в основном спектре

        # Суммируем энергию на основной частоте и 3 гармониках
        total_energy = 0
        for harmonic in range(1, 5):
            freq = harmonic * base_freq
            # Поиск в окрестности ±2% от частоты
            idx = np.argwhere((f > freq * 0.98) & (f < freq * 1.02)).flatten()

            if len(idx) > 0:
                # Берем максимальное значение в окрестности
                peak_energy = np.max(Pxx_env[idx])
                total_energy += peak_energy

        defect_levels[defect] = total_energy

    return defect_levels


def analyze_unbalance(x, fs):
    """Анализ дисбаланса по основному спектру"""
    nperseg = min(8192, len(x))
    f, Pxx = signal.welch(x, fs, nperseg=nperseg, scaling="spectrum")

    # Ищем пик на частоте вращения и ее гармониках
    fund_idx = np.argmin(np.abs(f - F_MOTOR))

    # Вычисляем энергию в окрестностях 1x, 2x, 3x частот
    unbalance_energy = 0
    for harmonic in range(1, 4):
        freq = harmonic * F_MOTOR
        idx = np.argmin(np.abs(f - freq))
        low_idx = max(0, idx - 10)
        high_idx = min(len(Pxx), idx + 11)
        band_energy = np.trapz(Pxx[low_idx:high_idx], dx=f[1] - f[0])
        unbalance_energy += band_energy

    # Отношение к общей энергии
    total_energy = np.trapz(Pxx, dx=f[1] - f[0])
    return unbalance_energy / total_energy if total_energy > 0 else 0


def analyze_misalignment(x, fs, segment):
    """Анализ расцентровки с учетом фазовых сдвигов"""
    # Спектральный анализ на частоте 2x
    nperseg = min(8192, len(x))
    f, Pxx = signal.welch(x, fs, nperseg=nperseg, scaling="spectrum")

    # Энергия на 2x частоте
    target_freq = 2 * F_MOTOR
    idx = np.argmin(np.abs(f - target_freq))
    low_idx = max(0, idx - 10)
    high_idx = min(len(Pxx), idx + 11)
    misalignment_energy = np.trapz(Pxx[low_idx:high_idx], dx=f[1] - f[0])

    # Фазовый анализ (если доступны все три фазы)
    phase_shift_score = 0
    if "S" in segment.columns and "T" in segment.columns:
        try:
            # Вычисление фазовых сдвигов
            r_phase = segment["R"].values
            s_phase = segment["S"].values
            t_phase = segment["T"].values

            # Корреляция между фазами
            corr_rs = np.correlate(r_phase, s_phase, mode="same")
            corr_rt = np.correlate(r_phase, t_phase, mode="same")

            # Разница в максимальной корреляции
            max_diff = np.abs(np.max(corr_rs) - np.max(corr_rt))
            phase_shift_score = min(max_diff / np.max(corr_rs), 1.0)
        except:
            phase_shift_score = 0

    # Комбинированный показатель (70% спектральной энергии, 30% фазового сдвига)
    misalignment_score = 0.7 * misalignment_energy + 0.3 * phase_shift_score

    return misalignment_score


def calculate_kurtosis(x):
    """Вычисление куртозиса сигнала и его огибающей"""
    # Куртозис исходного сигнала
    kurt_base = stats.kurtosis(x)

    # Куртозис огибающей
    analytic_signal = signal.hilbert(x)
    envelope = np.abs(analytic_signal)
    kurt_env = stats.kurtosis(envelope)

    return kurt_base, kurt_env


def process_segment(segment, file_idx, seg_idx, filename):
    """Обработка сегмента данных и извлечение признаков"""
    features = []
    labels = []
    defect_freqs = compute_defect_frequencies()

    # Для каждой фазы
    for phase in ["R", "S", "T"]:
        if phase not in segment.columns:
            continue

        try:
            # Преобразование данных
            x = pd.to_numeric(segment[phase], errors="coerce").dropna().values
            if len(x) < SEGMENT_LENGTH:
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

            # Анализ огибающей для подшипников
            bearing_defects = analyze_envelope(x, FS, defect_freqs)

            # Анализ дисбаланса
            unbalance_level = analyze_unbalance(x, FS)

            # Анализ расцентровки
            misalignment_level = analyze_misalignment(x, FS, segment)

            # Дополнительные признаки
            kurt_base, kurt_env = calculate_kurtosis(x)

            # Сохранение признаков и меток
            features.append([mean_val, std_val, skew_val, kurt_val, peak_val, crest_factor, kurt_env])
            labels.append(
                [
                    bearing_defects.get("outer_race", 0),
                    bearing_defects.get("inner_race", 0),
                    bearing_defects.get("rolling_element", 0),
                    bearing_defects.get("cage", 0),
                    unbalance_level,
                    misalignment_level,
                ]
            )

        except Exception as e:
            logger.error(f"Error processing {phase} phase in {filename} segment {seg_idx}: {e}")
            continue

    return features, labels


def normalize_defect_levels(labels_df):
    """Нормализация уровней дефектов с использованием квантилей"""
    normalized_df = labels_df.copy()

    for col in labels_df.columns:
        # Для каждого дефекта вычисляем 95-й процентиль
        q95 = normalized_df[col].quantile(0.95)

        if q95 > 0:
            # Нормализуем значения к диапазону [0, 1]
            normalized_df[col] = normalized_df[col] / q95

            # Ограничиваем максимальное значение 1.0
            normalized_df[col] = np.clip(normalized_df[col], 0, 1)
        else:
            # Если все значения нулевые, оставляем как есть
            normalized_df[col] = normalized_df[col]

    return normalized_df


def main():
    # Создаем директории для результатов
    os.makedirs("labeled_data", exist_ok=True)
    os.makedirs("debug_plots", exist_ok=True)

    # Получаем список файлов
    data_dir = "./Data_Set_main"
    all_features = []
    all_labels = []
    file_idx = 0

    # Список для сбора всех меток перед нормализацией
    raw_labels = []

    for filename in sorted(os.listdir(data_dir)):
        if filename.endswith(".csv"):
            filepath = os.path.join(data_dir, filename)
            print(f"\nProcessing {filename}... ({file_idx+1}/?)")
            file_idx += 1

            try:
                # Попытка чтения с заголовками
                try:
                    data = pd.read_csv(
                        filepath,
                        usecols=["current_R", "current_S", "current_T"],
                        low_memory=False,
                        dtype=float,
                        on_bad_lines="skip",
                    )
                    data = data.rename(columns={"current_R": "R", "current_S": "S", "current_T": "T"})
                except (ValueError, KeyError):
                    # Чтение без заголовков
                    data = pd.read_csv(
                        filepath,
                        header=None,
                        names=["R", "S", "T"],
                        usecols=[0, 1, 2],
                        low_memory=False,
                        dtype=float,
                        on_bad_lines="skip",
                    )

                # Удаление пустых столбцов
                data = data.dropna(axis=1, how="all")
                logger.info(f"Loaded {len(data)} samples, {len(data.columns)} phases")

                # Обработка сегментов
                n_segments = len(data) // SEGMENT_LENGTH
                logger.info(f"Processing {n_segments} segments")

                segment_features = []
                segment_labels = []

                for i in range(n_segments):
                    start_idx = i * SEGMENT_LENGTH
                    end_idx = (i + 1) * SEGMENT_LENGTH
                    segment = data.iloc[start_idx:end_idx].copy()

                    try:
                        features, labels = process_segment(segment, file_idx, i, filename)
                        if features and labels:
                            segment_features.extend(features)
                            segment_labels.extend(labels)
                    except Exception as e:
                        logger.error(f"Error processing segment {i}: {e}")

                logger.error(f"Extracted {len(segment_features)} records")
                all_features.extend(segment_features)
                raw_labels.extend(segment_labels)

            except Exception as e:
                logger.error(f"Error processing {filename}: {e}")

    # Создаем DataFrame с метками перед нормализацией
    labels_df = pd.DataFrame(
        raw_labels, columns=["outer_race", "inner_race", "rolling_element", "cage", "unbalance", "misalignment"]
    )

    # Сохранение сырых меток для анализа
    raw_labels_df = pd.DataFrame(
        raw_labels, columns=["outer_race", "inner_race", "rolling_element", "cage", "unbalance", "misalignment"]
    )
    raw_labels_df.to_csv("labeled_data/raw_defect_levels.csv", index=False)

    # Анализ распределения сырых меток
    logger.info(f"Raw defect levels distribution:\n{raw_labels_df.describe()}")

    # Визуализация распределения сырых меток
    raw_labels_df.hist(bins=50, figsize=(12, 8))
    plt.tight_layout()
    plt.savefig("labeled_data/raw_defect_levels_distribution.png")
    plt.close()

    # Нормализация меток
    normalized_labels = normalize_defect_levels(labels_df)

    # Анализ нормализованных меток
    print("\nNormalized defect levels distribution:")
    print(normalized_labels.describe())

    # Визуализация распределения нормализованных меток
    normalized_labels.hist(bins=50, figsize=(12, 8))
    plt.tight_layout()
    plt.savefig("labeled_data/normalized_defect_levels_distribution.png")
    plt.close()

    # Создаем финальный DataFrame
    features_df = pd.DataFrame(
        all_features, columns=["mean", "std", "skew", "kurtosis", "peak", "crest_factor", "kurtosis_env"]
    )

    full_df = pd.concat([features_df, normalized_labels], axis=1)

    # Сохранение размеченных данных
    full_df.to_csv("labeled_data/all_labeled.csv", index=False)
    print(f"\nLabeling completed. Saved {len(full_df)} records to labeled_data/all_labeled.csv")

    # Сохранение статистики
    with open("labeled_data/labeling_report.txt", "w") as f:
        f.write("=== Defect Labeling Report ===\n")
        f.write(f"Total segments processed: {len(full_df)}\n")
        f.write(f"Files processed: {file_idx}\n\n")

        f.write("=== Raw Defect Levels Statistics ===\n")
        f.write(raw_labels_df.describe().to_string())
        f.write("\n\n")

        f.write("=== Normalized Defect Levels Statistics ===\n")
        f.write(normalized_labels.describe().to_string())

    print("Saved labeling report to labeled_data/labeling_report.txt")


if __name__ == "__main__":
    main()
