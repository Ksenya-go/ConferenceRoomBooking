using FluentAssertions;
using ConferenceBooking.Domain.Common;

namespace ConferenceBooking.Domain.Tests.Common;

public class AggregateRootTests
{
    // Тестовий нащадок AggregateRoot, який дозволяє створити сутність із заданим Id
    private class TestAggregate : AggregateRoot
    {
        public TestAggregate(Guid id)
        {
            Id = id;
        }
    }

    // Тестовий тип сутності для перевірки, що різні типи з однаковим Id не вважаються рівними
    private class OtherTestAggregate : AggregateRoot
    {
        public OtherTestAggregate(Guid id)
        {
            Id = id;
        }
    }

    [Fact]
    public void Equals_SameTypeAndSameId_ReturnsTrue()
    {
        // Arrange: два різні об'єкти мають однаковий Id
        var id = Guid.NewGuid();

        var entity1 = new TestAggregate(id);
        var entity2 = new TestAggregate(id);

        // Act
        var result = entity1.Equals(entity2);
        var areEqualByOperator = entity1 == entity2;

        // Assert
        result.Should().BeTrue();
        areEqualByOperator.Should().BeTrue();
    }

    [Fact]
    public void Equals_SameTypeButDifferentId_ReturnsFalse()
    {
        // Arrange
        var entity1 = new TestAggregate(Guid.NewGuid());
        var entity2 = new TestAggregate(Guid.NewGuid());

        // Act
        var result = entity1.Equals(entity2);
        var areEqualByOperator = entity1 == entity2;

        // Assert
        result.Should().BeFalse();
        areEqualByOperator.Should().BeFalse();
    }

    [Fact]
    public void Equals_DifferentTypesWithSameId_ReturnsFalse()
    {
        // Arrange: Id однаковий, але типи сутностей різні
        var id = Guid.NewGuid();

        var entity1 = new TestAggregate(id);
        var entity2 = new OtherTestAggregate(id);

        // Act
        var result = entity1.Equals(entity2);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Equals_BothHaveEmptyId_ReturnsFalse()
    {
        // Arrange: дві нові сутності ще не мають справжнього Id
        var entity1 = new TestAggregate(Guid.Empty);
        var entity2 = new TestAggregate(Guid.Empty);

        // Act
        var result = entity1.Equals(entity2);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Equals_SameReference_ReturnsTrue()
    {
        // Arrange
        var entity = new TestAggregate(Guid.NewGuid());

        // Act
        var result = entity.Equals(entity);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Equals_NullObject_ReturnsFalse()
    {
        // Arrange
        var entity = new TestAggregate(Guid.NewGuid());

        // Act
        var result = entity.Equals(null);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_SameTypeAndId_ReturnsSameHashCode()
    {
        // Arrange: рівні сутності повинні мати однаковий HashCode
        var id = Guid.NewGuid();

        var entity1 = new TestAggregate(id);
        var entity2 = new TestAggregate(id);

        // Act
        var hashCode1 = entity1.GetHashCode();
        var hashCode2 = entity2.GetHashCode();

        // Assert
        hashCode1.Should().Be(hashCode2);
    }

    [Fact]
    public void GetHashCode_DifferentTypesWithSameId_ReturnDifferentHashCodes()
    {
        // Arrange
        var id = Guid.NewGuid();

        var entity1 = new TestAggregate(id);
        var entity2 = new OtherTestAggregate(id);

        // Act
        var hashCode1 = entity1.GetHashCode();
        var hashCode2 = entity2.GetHashCode();

        // Assert
        hashCode1.Should().NotBe(hashCode2);
    }
}