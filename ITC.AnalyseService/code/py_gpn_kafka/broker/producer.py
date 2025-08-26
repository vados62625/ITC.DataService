import datetime
from ssl import SSLContext

import aiokafka.errors
import gssapi
from aiokafka import AIOKafkaProducer
from loguru import logger

from py_gpn_kafka.broker.exceptions import RetryConnectionError
from py_gpn_kafka.broker.messages import MessageOut


class Producer:
    """A Kafka message producer for sending messages to a specified topic.

    Attributes:
        _connection_url (str): The URL for connecting to the Kafka broker.
        _topic (str): The topic to which messages will be produced.
        _use_krb (bool): Flag indicating whether Kerberos authentication should be used.
        _ssl_context (SSLContext): SSL context for secure connections.
        _principal (str): Kerberos principal for authentication.
        _keytab (str): Path to the keytab file for Kerberos authentication.
        _service_name (str): Kerberos service name.
    """

    def __init__(
        self,
        connection_url: str,
        topic: str,
        use_krb: bool = False,
        ssl_context: SSLContext = None,
        principal: str = None,
        keytab: str = None,
        service_name: str = None,
    ):
        """Initializes the Producer with connection details and topic.

        Args:
            connection_url (str): The URL of the Kafka broker to connect to.
            topic (str): The name of the Kafka topic to produce messages to.
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
        self._connection_url = connection_url
        self._topic = topic
        self._use_krb = use_krb
        self._ssl_context = ssl_context
        self._principal = principal
        self._keytab = keytab
        self._service_name = service_name
        self._producer = None
        self._min_gss_retry_interval_s = 30

    def _get_krb_args(self) -> dict:
        """Generates Kerberos authentication arguments for the Kafka producer.

        Performs kinit operation using the provided principal and keytab, and returns
        the necessary configuration for Kerberos authentication.

        Returns:
            A dictionary containing Kerberos configuration parameters for
            the Kafka producer. Returns empty dict if Kerberos is not properly
            configured.
        """

        logger.info("Attempting to enable kerberos for producer")
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

    async def produce_message(self, message: MessageOut) -> None:
        """Produces a message to the configured Kafka topic.

        Creates a producer instance, starts it, sends the message, and ensures proper
        cleanup. Handles exceptions during message production.

        Args:
            message: The message to be sent, containing key, value, and headers.

        Raises:
            Exception: Re-raises any exception that occurs during producer startup.
        """
        while True:
            last_try = datetime.datetime.now()
            try:
                self.__build_broker_producer()
                await self.__start_broker_producer()
                await self._producer.send(
                    topic=self._topic,
                    key=message.key,
                    value=message.value,
                    headers=message.headers,
                )
                await self._producer.flush()
                return
            except (gssapi.raw.misc.GSSError, aiokafka.errors.KafkaError) as error:
                logger.warning(f"Producer - got error {error}")
                second_from_last_try = (datetime.datetime.now() - last_try).total_seconds()
                if second_from_last_try < self._min_gss_retry_interval_s:
                    logger.error(
                        f"Minimum retry interval during handling producing message {self._topic} - {message.key}"
                    )
                    logger.error(f"Failed to produce message with {self._topic} - {message.key} due {error}")
                    await self._producer.stop()
                    raise RetryConnectionError(
                        f"produce messages by {self._topic} producer",
                        self._min_gss_retry_interval_s,
                        second_from_last_try,
                        error,
                    ) from error
                logger.info(f"Producer - attempting to restart {self._topic} producer")

            except Exception as e:
                logger.error(f"Failed to produce message with {self._topic} - {message.key} due {e}")
            finally:
                await self._producer.stop()

    async def __start_broker_producer(self) -> None:
        """Starts the Kafka producer instance.

        Args:
            broker_producer: The AIOKafkaProducer instance to start.

        Raises:
            Exception: If the producer fails to start, logs the error and re-raises.
        """
        try:
            await self._producer.start()
        except Exception as e:
            logger.error(f"Failed to start consumer due to {e}")
            raise e

    def __build_broker_producer(self) -> None:
        """Creates a new AIOKafkaProducer instance.

        Returns:
            A new producer instance configured with the connection URL.
        """
        if self._use_krb:
            self._producer = AIOKafkaProducer(
                bootstrap_servers=self._connection_url,
                ssl_context=self._ssl_context,
                **self._get_krb_args(),
            )
            return
        self._producer = AIOKafkaProducer(bootstrap_servers=self._connection_url)
