import os


class Settings:
    # SEGMENT_LENGTH = 65536
    SEGMENT_LENGTH = int(os.environ["SEGMENT_LENGTH"].strip())

    MODEL_PATH = os.environ["MODEL_PATH"].strip()

    TOPIC_NAME = os.environ["TOPIC_NAME"].strip()

    F_MOTOR = 1770 / 60


settings = Settings()
