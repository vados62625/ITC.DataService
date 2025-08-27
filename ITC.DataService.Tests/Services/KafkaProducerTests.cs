using Confluent.Kafka;
using FluentAssertions;
using ITC.DataService.Config;
using ITC.DataService.Services;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace ITC.DataService.Tests.Services;

[TestFixture]
public class KafkaProducerTests
{
    private Mock<IProducer<string, string>> _producerMock;
    private Mock<IOptions<KafkaProducerConfig>> _configMock;
    private KafkaProducer<string, string> _kafkaProducer;
    private KafkaProducerConfig _config;

    [SetUp]
    public void Setup()
    {
        _producerMock = new Mock<IProducer<string, string>>();
        _configMock = new Mock<IOptions<KafkaProducerConfig>>();
        _config = new KafkaProducerConfig { Topic = "test-topic" };
        _configMock.Setup(x => x.Value).Returns(_config);
        
        _kafkaProducer = new KafkaProducer<string, string>(_configMock.Object, _producerMock.Object);
    }

    [Test]
    public async Task ProduceAsync_WithValidKeyAndValue_ReturnsTrue()
    {
        // Arrange
        var key = "test-key";
        var value = "test-value";
        var deliveryResult = new DeliveryResult<string, string>
        {
            Status = PersistenceStatus.Persisted,
            Topic = "test-topic",
            Partition = 0,
            Offset = 123
        };

        _producerMock.Setup(x => x.ProduceAsync(
            It.IsAny<string>(), 
            It.IsAny<Message<string, string>>(), 
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(deliveryResult);

        // Act
        var result = await _kafkaProducer.ProduceAsync(key, value);

        // Assert
        result.Should().BeTrue();
        _producerMock.Verify(x => x.ProduceAsync(
            "test-topic", 
            It.Is<Message<string, string>>(m => m.Key == key && m.Value == value), 
            It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Test]
    public async Task ProduceAsync_WithNullKey_ReturnsTrue()
    {
        // Arrange
        string? key = null;
        var value = "test-value";
        var deliveryResult = new DeliveryResult<string, string>
        {
            Status = PersistenceStatus.Persisted,
            Topic = "test-topic",
            Partition = 0,
            Offset = 123
        };

        _producerMock.Setup(x => x.ProduceAsync(
            It.IsAny<string>(), 
            It.IsAny<Message<string, string>>(), 
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(deliveryResult);

        // Act
        var result = await _kafkaProducer.ProduceAsync(key, value);

        // Assert
        result.Should().BeTrue();
        _producerMock.Verify(x => x.ProduceAsync(
            "test-topic", 
            It.Is<Message<string, string>>(m => m.Key == null && m.Value == value), 
            It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Test]
    public async Task ProduceAsync_WithProduceException_ReturnsFalse()
    {
        // Arrange
        var key = "test-key";
        var value = "test-value";
        var produceException = new ProduceException<string, string>(
            new Error(ErrorCode.NoError), 
            new DeliveryResult<string, string>());

        _producerMock.Setup(x => x.ProduceAsync(
            It.IsAny<string>(), 
            It.IsAny<Message<string, string>>(), 
            It.IsAny<CancellationToken>()))
            .ThrowsAsync(produceException);

        // Act
        var result = await _kafkaProducer.ProduceAsync(key, value);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public async Task ProduceAsync_WithKafkaException_ReturnsFalse()
    {
        // Arrange
        var key = "test-key";
        var value = "test-value";
        var kafkaException = new KafkaException(new Error(ErrorCode.NoError));

        _producerMock.Setup(x => x.ProduceAsync(
            It.IsAny<string>(), 
            It.IsAny<Message<string, string>>(), 
            It.IsAny<CancellationToken>()))
            .ThrowsAsync(kafkaException);

        // Act
        var result = await _kafkaProducer.ProduceAsync(key, value);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public async Task ProduceAsync_WithNotPersistedStatus_ReturnsFalse()
    {
        // Arrange
        var key = "test-key";
        var value = "test-value";
        var deliveryResult = new DeliveryResult<string, string>
        {
            Status = PersistenceStatus.NotPersisted,
            Topic = "test-topic",
            Partition = 0,
            Offset = 123
        };

        _producerMock.Setup(x => x.ProduceAsync(
            It.IsAny<string>(), 
            It.IsAny<Message<string, string>>(), 
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(deliveryResult);

        // Act
        var result = await _kafkaProducer.ProduceAsync(key, value);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void Dispose_CallsProducerDispose()
    {
        // Act
        _kafkaProducer.Dispose();

        // Assert
        _producerMock.Verify(x => x.Dispose(), Times.Once);
    }

    [TearDown]
    public void TearDown()
    {
        _kafkaProducer?.Dispose();
    }
}
