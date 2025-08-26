import asyncio
import datetime
from ssl import SSLContext
from typing import NoReturn, Protocol

import aiokafka
import gssapi.raw.misc
from aiokafka import AIOKafkaConsumer
from loguru import logger

from py_gpn_kafka.broker.exceptions import RetryConnectionError
from py_gpn_kafka.broker.messages import MessageIn, MessageOut


class SupportsHandle(Protocol):
    """Protocol defining the interface for message handlers."""

    async def handle(self, message_in: MessageIn) -> MessageOut:
        """Process an incoming message and return a response.

        Args:
            message_in: The incoming message to process.

        Returns:
            The response message after processing.
        """
        ...


class WorkflowConsumer:
    """A Kafka consumer for workflow messages with pause/resume capability.

    Attributes:
        _name (str): The name of the consumer (includes topic when consuming).
        _connection_url (str): The URL for connecting to Kafka.
        _handler (SupportsHandle): The message handler implementing SupportsHandle protocol.
        _resume_event (asyncio.Event): Event to control pause/resume state.
        _idle_event (asyncio.Event): Event to track when consumer is idle.
        _use_krb (bool): Flag indicating whether Kerberos authentication should be used.
        _ssl_context (SSLContext): SSL context for secure connections.
        _principal (str): Kerberos principal for authentication.
        _keytab (str): Path to the keytab file for Kerberos authentication.
        _service_name (str): Kerberos service name.
        _consumer (AIOKafkaConsumer): AIOKafkaConsumer consumer instance.
    """

    def __init__(
        self,
        connection_url: str,
        handler: SupportsHandle,
        use_krb: bool = False,
        ssl_context: SSLContext = None,
        principal: str = None,
        keytab: str = None,
        service_name: str = None,
        consumer_kwargs: dict = None,
    ):
        """Initializes the WorkflowConsumer.

        Args:
            connection_url (str): The Kafka broker connection URL.
            handler (str): An object implementing the SupportsHandle protocol.
            use_krb (bool): Whether to use Kerberos authentication.
                Defaults to False.
            ssl_context (SSLContext): SSL context for secure connections.
                Defaults to None.
            principal (str): Kerberos principal for authentication.
                Defaults to None.
            keytab (str): Path to the keytab file for Kerberos.
                Defaults to None.
            service_name (str): Kerberos service name.
                Defaults to None.
        """
        self._name = "Consumer(unknown_topic)"
        self._connection_url = connection_url
        self._handler = handler
        self._resume_event = asyncio.Event()
        self._resume_event.set()
        self._idle_event = asyncio.Event()
        self._set_idle()
        self._use_krb = use_krb
        self._ssl_context = ssl_context
        self._principal = principal
        self._keytab = keytab
        self._service_name = service_name
        self._consumer = None
        self._consumer_kwargs = (
            consumer_kwargs if consumer_kwargs is not None else self._get_default_consumer_settings()
        )
        self._min_gss_retry_interval_s = 30

    def _get_default_consumer_settings(self) -> dict:
        return {
            "max_poll_records": 10,
            "max_poll_interval_ms": 60000,
            "session_timeout_ms": 10000,
            "heartbeat_interval_ms": 1000,
        }

    def _get_krb_args(self) -> dict:
        """Generates Kerberos authentication arguments for the Kafka consumer.

        Performs kinit operation using the provided principal and keytab, and returns
        the necessary configuration for Kerberos authentication.

        Returns:
            A dictionary containing Kerberos configuration parameters for
            the Kafka consumer. Returns empty dict if Kerberos is not properly
            configured.
        """

        logger.info("Attempting to enable kerberos for consumer")
        from krbticket import KrbCommand, KrbConfig

        if self._principal is None or self._keytab is None or self._service_name is None:
            return {}

        kconfig = KrbConfig(
            principal=self._principal,
            keytab=self._keytab,
        )
        KrbCommand.kinit(kconfig)
        return {
            "sasl_mechanism": "GSSAPI",
            "sasl_kerberos_service_name": self._service_name,
            "security_protocol": "SASL_SSL",
        }

    async def consume(self, topic: str) -> None:
        """Starts consuming messages from the specified topic.

        Args:
            topic: The Kafka topic to consume messages from.

        Raises:
            Exception: If consumer fails to start or handle messages.
        """
        self._name = f"Consumer({topic})"

        while True:
            last_try = datetime.datetime.now()
            try:
                await self._build_kafka_consumer(topic)
                await self._start_kafka_consumer()
                await self._handle_records()
            except (gssapi.raw.misc.GSSError, aiokafka.errors.KafkaError) as error:
                logger.warning(f"Consumer - got error {error}")
                second_from_last_try = (datetime.datetime.now() - last_try).total_seconds()
                if second_from_last_try < self._min_gss_retry_interval_s:
                    logger.error(f"{self._name} - minimum retry interval during handling messages")
                    await self._consumer.stop()
                    raise RetryConnectionError(
                        f"handle messages by {self._name} consumer",
                        self._min_gss_retry_interval_s,
                        second_from_last_try,
                        error,
                    ) from error

                logger.info(f"{self._name} - attempting to restart consumer")

            except Exception as e:
                logger.error(f"{self._name} - failed to consume messages due to {e}")
                await self._consumer.stop()
                raise e

    async def _build_kafka_consumer(self, topic: str):
        """Builds and returns an AIOKafkaConsumer instance.

        Args:
            topic: The topic to subscribe to.

        Returns:
            An initialized AIOKafkaConsumer instance.

        Raises:
            Exception: If consumer fails to build.
        """
        try:
            logger.info(f"Trying to build {self._name}")
            if self._use_krb:
                consumer = AIOKafkaConsumer(
                    topic,
                    bootstrap_servers=self._connection_url,
                    group_id=f"{topic}ConsumerGroup",
                    ssl_context=self._ssl_context,
                    **self._get_krb_args(),
                    **self._consumer_kwargs,
                )
            else:
                consumer = AIOKafkaConsumer(
                    topic,
                    bootstrap_servers=self._connection_url,
                    group_id=f"{topic}ConsumerGroup",
                    **self._consumer_kwargs,
                )
            logger.info(f"Successfully built {self._name}")
            self._consumer = consumer
        except Exception as e:
            logger.error(f"Failed to build {self._name} due to {e}")
            raise e

    async def _start_kafka_consumer(self) -> None:
        """Starts the Kafka consumer.

        Args:
            consumer: The AIOKafkaConsumer instance to start.

        Raises:
            Exception: If consumer fails to start.
        """
        try:
            logger.info(f"Starting {self._name}")
            await self._consumer.start()
            logger.info(f"Successfully started {self._name}")
        except Exception as e:
            logger.error(f"Failed to start {self._name} due to {e}")
            raise e

    async def _handle_records(self) -> NoReturn:
        """Continuously handles incoming records from the consumer.

        Args:
            consumer: The active AIOKafkaConsumer instance.

        Note:
            This method runs indefinitely until the consumer is stopped.
        """
        self._set_idle()
        if self._is_set_to_pause():
            await self._wait_to_resume()
        async for record in self._consumer:
            await self._wait_to_resume()
            self._set_active()
            message = MessageIn.from_record(record)
            logger.info(f"{self._name} - collect message {message.topic}")
            await self._handler.handle(message)
            logger.info(f"{self._name} - processed message {message.topic}")

            if self._is_set_to_pause():
                self._consumer.pause()
                self._set_idle()
                await self._wait_to_resume()
                self._consumer.resume()
            self._set_idle()

    def _set_idle(self) -> None:
        """Sets the consumer state to idle and logs the event."""
        self._idle_event.set()
        logger.info(f"{self._name} is idle now")

    def _set_active(self) -> None:
        """Sets the consumer state to active and logs the event."""
        self._idle_event.clear()
        logger.info(f"{self._name} is active now")

    def _is_set_to_pause(self) -> bool:
        """Checks if consumer is set to pause.

        Returns:
            True if consumer should pause, False otherwise.
        """
        return not self._resume_event.is_set()

    async def _wait_to_resume(self) -> None:
        """Waits for resume signal if consumer is paused."""
        logger.info(f"{self._name} wait for resume now")
        await self._resume_event.wait()
        if await self.is_idle():
            logger.info(f"{self._name} is idle now")

    async def pause(self) -> None:
        """Pauses message consumption after current message completes.

        Waits for consumer to become idle before returning.
        """
        self._resume_event.clear()
        await self._idle_event.wait()
        logger.info(f"{self._name} is paused now")

    async def resume(self) -> None:
        """Resumes message consumption."""
        self._resume_event.set()
        logger.info(f"{self._name} is resumed now")

    async def is_idle(self) -> bool:
        """Checks if consumer is currently idle.

        Returns:
            True if consumer is idle, False otherwise.
        """
        return self._idle_event.is_set()
