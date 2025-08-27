using FluentAssertions;
using ITC.Domain.Models;
using NUnit.Framework;

namespace ITC.DataService.Tests.Models;

[TestFixture]
public class EntityBaseTests
{
    [Test]
    public void EntityBase_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var entity = new TestEntity();

        // Assert
        entity.Id.Should().Be(Guid.Empty); // EntityBase по умолчанию имеет Guid.Empty
        entity.CreatedAt.Should().Be(DateTimeOffset.MinValue); // EntityBase по умолчанию имеет MinValue
        entity.UpdatedAt.Should().BeNull();
        entity.DeletedAt.Should().BeNull();
        entity.Version.Should().Be(0);
        entity.LockToken.Should().Be(0);
        entity.Timestamp.Should().Be(0);
    }

    [Test]
    public void EntityBase_Properties_CanBeSetAndRetrieved()
    {
        // Arrange
        var id = Guid.NewGuid();
        var createdAt = DateTimeOffset.UtcNow.AddDays(-1);
        var updatedAt = DateTimeOffset.UtcNow;
        var deletedAt = DateTimeOffset.UtcNow.AddDays(1);
        var version = (uint)123;
        var lockToken = 456L;
        var timestamp = 789L;

        // Act
        var entity = new TestEntity
        {
            Id = id,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt,
            DeletedAt = deletedAt,
            Version = version,
            LockToken = lockToken,
            Timestamp = timestamp
        };

        // Assert
        entity.Id.Should().Be(id);
        entity.CreatedAt.Should().Be(createdAt);
        entity.UpdatedAt.Should().Be(updatedAt);
        entity.DeletedAt.Should().Be(deletedAt);
        entity.Version.Should().Be(version);
        entity.LockToken.Should().Be(lockToken);
        entity.Timestamp.Should().Be(timestamp);
    }

    [Test]
    public void EntityBase_ImplementsIEntity()
    {
        // Arrange & Act
        var entity = new TestEntity();

        // Assert
        entity.Should().BeAssignableTo<IEntity>();
    }

    [Test]
    public void EntityBase_ImplementsITimeStampedModel()
    {
        // Arrange & Act
        var entity = new TestEntity();

        // Assert
        entity.Should().BeAssignableTo<ITimeStampedModel>();
    }

    [Test]
    public void EntityBase_Properties_AreMutable()
    {
        // Arrange
        var entity = new TestEntity();
        var newId = Guid.NewGuid();
        var newUpdatedAt = DateTimeOffset.UtcNow;

        // Act
        entity.Id = newId;
        entity.UpdatedAt = newUpdatedAt;

        // Assert
        entity.Id.Should().Be(newId);
        entity.UpdatedAt.Should().Be(newUpdatedAt);
    }

    [Test]
    public void EntityBase_CanBeNullified()
    {
        // Arrange
        var entity = new TestEntity
        {
            UpdatedAt = DateTimeOffset.UtcNow,
            DeletedAt = DateTimeOffset.UtcNow
        };

        // Act
        entity.UpdatedAt = null;
        entity.DeletedAt = null;

        // Assert
        entity.UpdatedAt.Should().BeNull();
        entity.DeletedAt.Should().BeNull();
    }

    // Test implementation of abstract class
    private class TestEntity : EntityBase
    {
    }
}
