from py_gpn_kafka.broker.handlers.base_handler import BaseHandler
from py_gpn_kafka.broker.messages import MessageIn
from src.models import InputData, Defects
from src.processing.processing import ProcessData


class DataHandler(BaseHandler[Defects]):
    def __init__(self, process_data: ProcessData):
        self.process_data = process_data

    async def process_business_logic(self, message_in: MessageIn) -> Defects:
        input_bytes = message_in.value
        input_data = InputData.model_validate_json(input_bytes)
        defects = self.process_data.predict(data=input_data.data)
        return Defects(defects=defects, file_id=input_data.FileId, datetime=input_data.DateTime)
