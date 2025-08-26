from functools import cached_property

from py_gpn_kafka import BrokerConfig, TopicInfo
from src.common.config import settings
from src.api.broker.data_handler import DataHandler
from src.processing.structure import ProcessStructure


class BrokerStructure:
    def __init__(self, process_structure: ProcessStructure):
        self.process_structure = process_structure

    @cached_property
    def topics_info(self) -> list[TopicInfo]:
        topic_name = settings.TOPIC_NAME
        return [
            TopicInfo(
                topic_name=topic_name,
                handler_cls=DataHandler,
                handler_kwargs={
                    "process_data": self.process_structure.process_data,
                },
            )
        ]

    @cached_property
    def broker_config(self) -> BrokerConfig:
        return BrokerConfig(topics_info=self.topics_info)
