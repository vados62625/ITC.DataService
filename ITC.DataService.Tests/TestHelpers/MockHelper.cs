using Microsoft.Extensions.Logging;
using Moq;

namespace ITC.DataService.Tests.TestHelpers;

public static class MockHelper
{
    public static Mock<ILogger<T>> CreateLoggerMock<T>()
    {
        var mock = new Mock<ILogger<T>>();
        
        // Setup basic logging methods
        mock.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
        
        return mock;
    }

    public static Mock<ILogger<T>> CreateLoggerMock<T>(LogLevel minimumLevel)
    {
        var mock = CreateLoggerMock<T>();
        mock.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns((LogLevel level) => level >= minimumLevel);
        return mock;
    }

    public static void VerifyLog<T>(Mock<ILogger<T>> loggerMock, LogLevel level, string message)
    {
        loggerMock.Verify(
            x => x.Log(
                level,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(message)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }

    public static void VerifyLog<T>(Mock<ILogger<T>> loggerMock, LogLevel level, Func<string, bool> messagePredicate)
    {
        loggerMock.Verify(
            x => x.Log(
                level,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => messagePredicate(v.ToString())),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }
}
