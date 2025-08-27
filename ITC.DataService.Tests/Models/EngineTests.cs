using FluentAssertions;
using ITC.Domain.Enums;
using ITC.Domain.Models;
using NUnit.Framework;

namespace ITC.DataService.Tests.Models;

[TestFixture]
public class EngineTests
{
    [Test]
    public void Engine_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var engine = new Engine();

        // Assert
        engine.Name.Should().BeNull();
        engine.EngineStatus.Should().Be(EngineStatus.New);
        engine.EngineType.Should().Be(EngineType.Live);
        engine.Analyses.Should().NotBeNull();
        engine.Analyses.Should().BeEmpty();
    }

    [Test]
    public void Engine_Properties_CanBeSetAndRetrieved()
    {
        // Arrange
        var name = "Test Engine";
        var status = EngineStatus.Success;
        var type = EngineType.FromFile;
        var analyses = new List<Analysis> { new Analysis() };

        // Act
        var engine = new Engine
        {
            Name = name,
            EngineStatus = status,
            EngineType = type,
            Analyses = analyses
        };

        // Assert
        engine.Name.Should().Be(name);
        engine.EngineStatus.Should().Be(status);
        engine.EngineType.Should().Be(type);
        engine.Analyses.Should().BeEquivalentTo(analyses);
    }

    [Test]
    public void Engine_InheritsFromEntityBase()
    {
        // Arrange & Act
        var engine = new Engine();

        // Assert
        engine.Should().BeAssignableTo<EntityBase>();
    }

    [Test]
    public void Engine_Analyses_CanBeModified()
    {
        // Arrange
        var engine = new Engine();
        var analysis1 = new Analysis();
        var analysis2 = new Analysis();

        // Act
        engine.Analyses.Add(analysis1);
        engine.Analyses.Add(analysis2);

        // Assert
        engine.Analyses.Should().HaveCount(2);
        engine.Analyses.Should().Contain(analysis1);
        engine.Analyses.Should().Contain(analysis2);
    }

    [Test]
    public void Engine_Analyses_CanBeCleared()
    {
        // Arrange
        var engine = new Engine();
        engine.Analyses.Add(new Analysis());
        engine.Analyses.Add(new Analysis());

        // Act
        engine.Analyses.Clear();

        // Assert
        engine.Analyses.Should().BeEmpty();
    }

    [Test]
    public void Engine_CanBeNullified()
    {
        // Arrange
        var engine = new Engine
        {
            Name = "Test Engine",
            Analyses = new List<Analysis> { new Analysis() }
        };

        // Act
        engine.Name = null;
        engine.Analyses = new List<Analysis>();

        // Assert
        engine.Name.Should().BeNull();
        engine.Analyses.Should().BeEmpty();
    }

    [Test]
    public void Engine_WithAllPropertiesSet_IsValid()
    {
        // Arrange
        var id = Guid.NewGuid();
        var createdAt = DateTimeOffset.UtcNow.AddDays(-1);
        var name = "High Performance Engine";
        var status = EngineStatus.Pending;
        var type = EngineType.Live;

        // Act
        var engine = new Engine
        {
            Id = id,
            CreatedAt = createdAt,
            Name = name,
            EngineStatus = status,
            EngineType = type
        };

        // Assert
        engine.Id.Should().Be(id);
        engine.CreatedAt.Should().Be(createdAt);
        engine.Name.Should().Be(name);
        engine.EngineStatus.Should().Be(status);
        engine.EngineType.Should().Be(type);
        engine.Analyses.Should().NotBeNull();
        engine.Analyses.Should().BeEmpty();
    }
}
