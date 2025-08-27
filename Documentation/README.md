# Unit Tests для ITC.DataService

Этот проект содержит unit тесты для основных компонентов системы ITC.DataService, написанные с использованием NUnit.

## Структура тестов

### Services
- **CsvServiceTests** - тесты для сервиса обработки CSV файлов
- **KafkaProducerTests** - тесты для Kafka продюсера

### Models  
- **EntityBaseTests** - тесты для базовой сущности
- **EngineTests** - тесты для модели Engine

### Integration
- **CsvServiceIntegrationTests** - интеграционные тесты для CsvService

### TestHelpers
- **MockHelper** - вспомогательные методы для создания моков

## Зависимости

- **NUnit 4.0.1** - фреймворк для unit тестирования
- **Moq 4.20.70** - библиотека для создания моков
- **FluentAssertions 6.12.0** - fluent API для assertions
- **coverlet.collector 6.0.0** - сбор метрик покрытия кода

## Запуск тестов

### Через Visual Studio/Rider
1. Откройте Solution Explorer
2. Правый клик на проекте `ITC.DataService.Tests`
3. Выберите "Run Tests"

### Через командную строку
```bash
# Из корня solution
dotnet test ITC.DataService.Tests/ITC.DataService.Tests.csproj

# Или из директории с тестами
cd ITC.DataService.Tests
dotnet test

# С подробным выводом
dotnet test --verbosity normal

# С покрытием кода
dotnet test --collect:"XPlat Code Coverage"
```

### Запуск конкретных тестов
```bash
# Из корня solution
dotnet test ITC.DataService.Tests/ITC.DataService.Tests.csproj --filter Category=Integration

# Или из директории с тестами
cd ITC.DataService.Tests
dotnet test --filter Category=Integration

# Запуск конкретного теста
dotnet test --filter "FullyQualifiedName~CsvServiceTests"
```

## Покрытие кода

Для анализа покрытия кода используйте:
```bash
dotnet test --collect:"XPlat Code Coverage"
```

Результаты будут сохранены в папке `TestResults`.

## Написание новых тестов

### Структура теста
```csharp
[Test]
public void MethodName_Scenario_ExpectedResult()
{
    // Arrange - подготовка данных
    var service = new TestService();
    
    // Act - выполнение действия
    var result = service.Method();
    
    // Assert - проверка результата
    result.Should().NotBeNull();
}
```

### Использование моков
```csharp
[SetUp]
public void Setup()
{
    _mockService = new Mock<ITestService>();
    _service = new Service(_mockService.Object);
}

[Test]
public void Test_WithMock_VerifiesInteraction()
{
    // Arrange
    _mockService.Setup(x => x.Method()).Returns("test");
    
    // Act
    var result = _service.Process();
    
    // Assert
    _mockService.Verify(x => x.Method(), Times.Once);
}
```

### Асинхронные тесты
```csharp
[Test]
public async Task AsyncMethod_Scenario_ReturnsExpectedResult()
{
    // Arrange
    var service = new AsyncService();
    
    // Act
    var result = await service.AsyncMethod();
    
    // Assert
    result.Should().Be("expected");
}
```

## Лучшие практики

1. **Именование тестов**: `MethodName_Scenario_ExpectedResult`
2. **AAA Pattern**: Arrange, Act, Assert
3. **Один тест - одна проверка**: каждый тест должен проверять одну вещь
4. **Используйте моки**: для изоляции тестируемого кода
5. **Группируйте тесты**: используйте `[Category]` для группировки
6. **Тестируйте граничные случаи**: пустые значения, null, исключения

## Troubleshooting

### Ошибка "Assembly not found"
Убедитесь, что проект `ITC.DataService.Tests` добавлен в solution и имеет ссылки на тестируемые проекты.

### Ошибка "Package not found"
Выполните восстановление NuGet пакетов:
```bash
dotnet restore
```

### Тесты не запускаются
Проверьте, что:
1. Целевая платформа совпадает (.NET 9.0)
2. Все зависимости установлены
3. Проект успешно собирается
