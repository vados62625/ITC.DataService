from abc import ABC, abstractmethod
from typing import Generic, TypeVar

from loguru import logger
from pydantic import BaseModel, ValidationError

from py_gpn_kafka.broker.messages import MessageIn, MessageOut
from py_gpn_kafka.broker.presenters import BasePresenter
from py_gpn_kafka.broker.producer import Producer
from py_gpn_kafka.models.deserializers import TaskId
from py_gpn_kafka.models.errors import MSGError

OutputT = TypeVar("OutputT", bound=BaseModel)


class BaseHandler(Generic[OutputT], ABC):
    """Abstract base class for message handlers.

    Provides common functionality for processing messages including validation,
    execution, error handling, and response production.

    Attributes:
        producer: Producer instance for sending output messages.
        presenter: Presenter instance for formatting output messages.
    """

    producer: Producer = None
    presenter: BasePresenter = None

    def init_base(self, producer: Producer, presenter: BasePresenter) -> None:
        """Initializes the base handler with required dependencies.

        Args:
            producer: Kafka message producer instance.
            presenter: Message presentation/formatter instance.
        """
        self.producer = producer
        self.presenter = presenter

    async def handle(self, message_in: MessageIn) -> MessageOut:
        """Processes an incoming message and produces an appropriate response.

        The method handles the complete message lifecycle:
        1. Initializes a default error response
        2. Attempts to validate and process the message
        3. Catches and handles any exceptions
        4. Produces an output message with appropriate status headers
        5. Returns the output message

        Args:
            message_in: The incoming message to process.

        Returns:
            The response message with status headers.
        """
        response = MSGError()
        value = self.presenter.present(response)
        headers = [("status", b"error")]
        try:
            response = await self.execute(message_in)
            logger.info(f"Response has been received")
            value = self.presenter.present(response)
            headers = [("status", b"success"), ("msg", b"")]
        except (Exception, ValidationError) as e:
            value = self.presenter.present(response)
            error_msg = f"Failed handle message" f"due {type(e).__name__} with error message: {str(e)}"
            logger.error(error_msg)
            response.error_msg = error_msg
            headers = [headers[0], ("msg", bytes(str(e)))]
        finally:
            message_out = MessageOut(
                key=None,
                value=value,
                headers=headers,
            )
            await self.producer.produce_message(message_out)
            return message_out

    @abstractmethod
    async def process_business_logic(self, message_in: MessageIn) -> OutputT:
        """Abstract method containing business logic for message processing.

        Args:
            message_in: The incoming message to process.

        Returns:
            Processed output as a Pydantic model.
        """
        ...

    async def execute(self, message_in: MessageIn) -> OutputT:
        """Execute the message processing pipeline.

        Args:
            message_in: The incoming message to process.

        Returns:
            The response object to be presented and sent as output.

        Raises:
            Exception: Any exception that occurs during processing will be caught
                and handled by the `handle` method.
        """
        logger.info(f"Starting processing message")
        return await self.process_business_logic(message_in)
