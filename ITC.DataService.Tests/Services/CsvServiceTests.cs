using System.Text;
using CsvHelper;
using FluentAssertions;
using ITC.DataService.Services;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace ITC.DataService.Tests.Services;

[TestFixture]
public class CsvServiceTests
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
    public async Task ProcessCsvToJsonAsync_WithValidCsv_ReturnsValidJson()
    {
        // Arrange
        var csvContent = "Column1;Column2;Column3\nValue1;Value2;Value3\nValue4;Value5;Value6";
        var csvStream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

        // Act
        var result = await _csvService.ProcessCsvToJsonAsync(csvStream);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Contain("Column1");
        result.Should().Contain("Value1");
        result.Should().Contain("Column2");
        result.Should().Contain("Value2");
    }

    [Test]
    public async Task GetColumnsData_WithValidCsv_ReturnsCorrectStructure()
    {
        // Arrange
        var csvContent = "Name;Age;City\nJohn;25;Moscow\nJane;30;SPb";
        var csvStream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

        // Act
        var result = await _csvService.GetColumnsData(csvStream);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().ContainKey("Name");
        result.Should().ContainKey("Age");
        result.Should().ContainKey("City");
        
        result["Name"].Should().HaveCount(2);
        result["Name"].Should().Contain("John");
        result["Name"].Should().Contain("Jane");
        
        result["Age"].Should().HaveCount(2);
        result["Age"].Should().Contain("25");
        result["Age"].Should().Contain("30");
    }

    [Test]
    public void GetColumnsData_WithEmptyCsv_ThrowsException()
    {
        // Arrange
        var csvContent = "";
        var csvStream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

        // Act & Assert
        var exception = Assert.ThrowsAsync<CsvHelper.ReaderException>(async () => 
            await _csvService.GetColumnsData(csvStream));
        
        exception.Message.Should().Contain("No header record was found");
    }

    [Test]
    public async Task GetColumnsData_WithCsvWithoutData_ReturnsEmptyLists()
    {
        // Arrange
        var csvContent = "Header1;Header2;Header3";
        var csvStream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

        // Act
        var result = await _csvService.GetColumnsData(csvStream);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result["Header1"].Should().BeEmpty();
        result["Header2"].Should().BeEmpty();
        result["Header3"].Should().BeEmpty();
    }

    [Test]
    public async Task GetColumnsData_WithCsvContainingEmptyValues_SkipsEmptyValues()
    {
        // Arrange
        var csvContent = "Col1;Col2;Col3\nValue1;;Value3\n;Value2;";
        var csvStream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

        // Act
        var result = await _csvService.GetColumnsData(csvStream);

        // Assert
        result["Col1"].Should().HaveCount(1);
        result["Col1"].Should().Contain("Value1");
        
        result["Col2"].Should().HaveCount(1);
        result["Col2"].Should().Contain("Value2");
        
        result["Col3"].Should().HaveCount(1);
        result["Col3"].Should().Contain("Value3");
    }

    [Test]
    public void GetRowsData_WithValidCsv_ReturnsCorrectRows()
    {
        // Arrange
        var csvContent = "current_R,current_S,current_T\n1.5,2.3,3.1\n4.2,5.7,6.9";
        var csvStream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

        // Act
        var result = _csvService.GetRowsData<object>(csvStream);

        // Assert
        result.Should().NotBeNull();
        var rows = result.ToList();
        rows.Should().HaveCount(2);
        
        // First row
        rows[0].Should().HaveCount(3);
        rows[0][0].Should().Be("1.5");
        rows[0][1].Should().Be("2.3");
        rows[0][2].Should().Be("3.1");
        
        // Second row
        rows[1].Should().HaveCount(3);
        rows[1][0].Should().Be("4.2");
        rows[1][1].Should().Be("5.7");
        rows[1][2].Should().Be("6.9");
    }

    [Test]
    public void GetRowsData_WithEmptyCsv_ReturnsEmptyResult()
    {
        // Arrange
        var csvContent = "current_R,current_S,current_T";
        var csvStream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

        // Act
        var result = _csvService.GetRowsData<object>(csvStream);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
}
