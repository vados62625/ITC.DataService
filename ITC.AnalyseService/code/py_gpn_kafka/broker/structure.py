import ssl
from functools import cached_property
from typing import Optional
from loguru import logger

from aiokafka.helpers import create_ssl_context

from py_gpn_kafka.broker.consumer import WorkflowConsumer
from py_gpn_kafka.broker.handlers import BaseHandler
from py_gpn_kafka.broker.presenters import BasePresenter, ResponseJsonBytesPresenter
from py_gpn_kafka.broker.producer import Producer
from py_gpn_kafka.core.config import BrokerConfig


def build_topic(prefix: str, name: str, suffix: Optional[str] = None) -> str:
    """Constructs a Kafka topic name from components.

    Args:
        prefix: The prefix for the topic.
        name: The main name for the topic.
        suffix: Optional suffix to append to the topic name.

    Returns:
        The constructed topic name.
    """
    if not prefix and not suffix:
        return name
    if not prefix:
        return f"{name}{suffix}"
    if not suffix:
        return f"{prefix}_{name}"
    return f"{prefix}{name}{suffix}"


class ApiStructure:
    """A class for creating Kafka-related components with consistent configuration.

    This class provides methods to create topics, producers, handlers, and consumers
    using a shared BrokerConfig.

    Attributes:
        broker_config: The configuration object containing Kafka connection details
            and naming conventions.
    """

    def __init__(self, broker_config: BrokerConfig) -> None:
        """Initializes the ApiStructure with the given broker configuration.

        Args:
            broker_config: The configuration object for Kafka broker settings.
        """
        self.broker_config = broker_config

    def __build_disabled_ssl_context(self):
        ssl_context = ssl.create_default_context(ssl.Purpose.SERVER_AUTH)
        ssl_context.options |= ssl.OP_NO_SSLv2 | ssl.OP_NO_SSLv3
        return ssl_context

    @cached_property
    def consumer_ssl_context(self):
        if self.broker_config.CA_FILE is None:
            logger.warning("Disabling ssl for kfk consumer")
            return self.__build_disabled_ssl_context()
        ssl_context = create_ssl_context(cafile=self.broker_config.CA_FILE)
        ssl_context.options |= ssl.OP_NO_TLSv1 | ssl.OP_NO_TLSv1_1
        return ssl_context

    @cached_property
    def producer_ssl_context(self):
        if self.broker_config.CA_FILE is None:
            logger.warning("Disabling ssl for kfk consumer")
            return self.__build_disabled_ssl_context()
        ssl_context = create_ssl_context(cafile=self.broker_config.CA_FILE)
        ssl_context.options |= ssl.OP_NO_TLSv1 | ssl.OP_NO_TLSv1_1
        return ssl_context

    def create_topic(self, topic_name: str, is_response: bool = False) -> str:
        """Creates a properly formatted topic name using the broker configuration.

        Args:
            topic_name: The base name for the topic.
            is_response: Whether this is a response topic (will use response suffix from config if True).

        Returns:
            The fully formatted topic name.
        """
        return build_topic(
            self.broker_config.TOPIC_PREFIX,
            topic_name,
            self.broker_config.RESPONSE_TOPIC_SUFFIX if is_response else None,
        )

    def create_producer(self, response_topic: str) -> Producer:
        """Creates a Kafka producer instance.

        Args:
            response_topic: The topic name this producer should publish to.

        Returns:
            A configured Producer instance.
        """
        krb_kwargs = {}
        if self.broker_config.USE_KRB:
            krb_kwargs = {
                "use_krb": True,
                "ssl_context": self.producer_ssl_context,
                "principal": self.broker_config.PRINCIPAL,
                "keytab": self.broker_config.KEYTAB,
                "service_name": self.broker_config.SERVICE_NAME,
            }

        return Producer(connection_url=self.broker_config.KAFKA_URL, topic=response_topic, **krb_kwargs)

    def create_handler(
        self,
        handler_cls: type(BaseHandler),
        producer: Producer,
        presenter: type(BasePresenter) = ResponseJsonBytesPresenter,
        *args,
        **kwargs,
    ) -> BaseHandler:
        """Creates a message handler instance.

        Args:
            handler_cls: The handler class to instantiate.
            producer: The producer instance the handler should use.
            presenter: The presenter class to use for formatting responses.
                Defaults to ResponseJsonBytesPresenter.
            *args: Additional positional arguments to pass to the handler
                constructor.
            **kwargs: Additional keyword arguments to pass to the handler
                constructor.

        Returns:
            An initialized handler instance.
        """

        handler = handler_cls(*args, **kwargs)
        handler.init_base(producer=producer, presenter=presenter())
        return handler

    def create_consumer(self, handler: BaseHandler) -> WorkflowConsumer:
        """Creates a Kafka consumer instance.

        Args:
            handler: The handler instance that will process consumed messages.

        Returns:
            A configured WorkflowConsumer instance.
        """
        krb_kwargs = {}
        if self.broker_config.USE_KRB:
            krb_kwargs = {
                "use_krb": True,
                "ssl_context": self.consumer_ssl_context,
                "principal": self.broker_config.PRINCIPAL,
                "keytab": self.broker_config.KEYTAB,
                "service_name": self.broker_config.SERVICE_NAME,
            }
        return WorkflowConsumer(connection_url=self.broker_config.KAFKA_URL, handler=handler, **krb_kwargs)
