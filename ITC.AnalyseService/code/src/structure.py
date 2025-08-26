from src.api.broker.structure import BrokerStructure
from src.processing.structure import ProcessStructure


class Structure:
    def __init__(self, broker_structure: BrokerStructure):
        self.broker_structure = broker_structure


_process_structure = ProcessStructure()
_broker_structure = BrokerStructure(process_structure=_process_structure)

structure = Structure(broker_structure=_broker_structure)
