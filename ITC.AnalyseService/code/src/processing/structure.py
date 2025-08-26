from functools import cached_property

from src.processing.processing import ProcessData


class ProcessStructure:
    @cached_property
    def process_data(self) -> ProcessData:
        return ProcessData()
