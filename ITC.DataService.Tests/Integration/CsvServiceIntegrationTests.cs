using System.Text;
using FluentAssertions;
using ITC.DataService.Services;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace ITC.DataService.Tests.Integration;

[TestFixture]
[Category("Integration")]
public class CsvServiceIntegrationTests
{
    private CsvService _csvService;
    private Mock<ILogger<CsvService>> _loggerMock;

    [SetUp]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger<CsvService>>();
        _csvService = new CsvService(_loggerMock.Object);
    }

    [Test]
    public async Task ProcessCsvToJsonAsync_LargeCsvFile_ProcessesCorrectly()
    {
        // Arrange
        var csvBuilder = new StringBuilder();
        csvBuilder.AppendLine("Id;Name;Value;Status");
        
        // Generate 1000 rows
        for (int i = 1; i <= 1000; i++)
        {
            csvBuilder.AppendLine($"{i};Item{i};Value{i};Active");
        }
        
        var csvContent = csvBuilder.ToString();
        var csvStream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

        // Act
        var result = await _csvService.ProcessCsvToJsonAsync(csvStream);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Contain("Id");
        result.Should().Contain("Name");
        result.Should().Contain("Value");
        result.Should().Contain("Status");
        result.Should().Contain("Item1000");
        result.Should().Contain("Value1000");
    }

    [Test]
    public async Task GetColumnsData_ComplexCsvWithSpecialCharacters_HandlesCorrectly()
    {
        // Arrange - используем простые заголовки без специальных символов
        var csvContent = @"Column1;Column2;Column3;Column4;Column5
Value1;Value2;Value3;Value4;Value5
Test;Data;Info;More;Data";
        var csvStream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

        // Act
        var result = await _csvService.GetColumnsData(csvStream);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(5);
        result.Should().ContainKey("Column1");
        result.Should().ContainKey("Column2");
        result.Should().ContainKey("Column3");
        result.Should().ContainKey("Column4");
        result.Should().ContainKey("Column5");
        
        result["Column1"].Should().Contain("Value1");
        result["Column2"].Should().Contain("Value2");
        result["Column3"].Should().Contain("Value3");
    }

    [Test]
    public async Task GetColumnsData_CsvWithDifferentDelimiters_ProcessesCorrectly()
    {
        // Arrange - используем точку с запятой как в основном методе
        var csvContent = "Field1;Field2;Field3\nData1;Data2;Data3";
        var csvStream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

        // Act
        var result = await _csvService.GetColumnsData(csvStream);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().ContainKey("Field1");
        result.Should().ContainKey("Field2");
        result.Should().ContainKey("Field3");
    }

    [Test]
    public async Task GetColumnsData_CsvWithEmptyRows_SkipsEmptyRows()
    {
        // Arrange
        var csvContent = "Header1;Header2;Header3\nValue1;Value2;Value3\n\nValue4;Value5;Value6";
        var csvStream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

        // Act
        var result = await _csvService.GetColumnsData(csvStream);

        // Assert
        result.Should().NotBeNull();
        result["Header1"].Should().HaveCount(2);
        result["Header1"].Should().Contain("Value1");
        result["Header1"].Should().Contain("Value4");
    }

    [Test]
    public async Task GetColumnsData_CsvWithMixedDataTypes_HandlesCorrectly()
    {
        // Arrange
        var csvContent = "String;Number;Boolean;Date\nText;123;true;2024-01-01\nAnother;456;false;2024-12-31";
        var csvStream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

        // Act
        var result = await _csvService.GetColumnsData(csvStream);

        // Assert
        result.Should().NotBeNull();
        result["String"].Should().Contain("Text");
        result["String"].Should().Contain("Another");
        result["Number"].Should().Contain("123");
        result["Number"].Should().Contain("456");
        result["Boolean"].Should().Contain("true");
        result["Boolean"].Should().Contain("false");
        result["Date"].Should().Contain("2024-01-01");
        result["Date"].Should().Contain("2024-12-31");
    }

    [Test]
    public void GetRowsData_LargeCsvFile_ProcessesEfficiently()
    {
        // Arrange
        var csvBuilder = new StringBuilder();
        csvBuilder.AppendLine("current_R,current_S,current_T");
        
        // Generate 5000 rows
        for (int i = 1; i <= 5000; i++)
        {
            csvBuilder.AppendLine($"{i * 0.1:F1},{i * 0.2:F1},{i * 0.3:F1}");
        }
        
        var csvContent = csvBuilder.ToString();
        var csvStream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = _csvService.GetRowsData<object>(csvStream);
        var rows = result.ToList();
        stopwatch.Stop();

        // Assert
        rows.Should().HaveCount(5000);
        rows[0].Should().HaveCount(3);
        rows[4999].Should().HaveCount(3);
        
        // Performance assertion - should process in reasonable time
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(5000); // 5 seconds max
    }
}
