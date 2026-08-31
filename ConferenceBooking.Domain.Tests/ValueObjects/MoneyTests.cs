using ConferenceBooking.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace ConferenceBooking.Domain.Tests.ValueObjects;

public class MoneyTests
{
    [Fact]
    public void Uah_NegativeAmount_ThrowsArgumentException()
    {
        // Arrange
        var act = () => Money.Uah(-100);

        // Act & Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Uah_ZeroAmount_DoesNotThrow()
    {
        // Arrange
        var act = () => Money.Uah(0);

        // Act & Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Add_SameCurrency_ReturnsSummedAmount()
    {
        // Arrange
        var money = Money.Uah(1000);
        var otherMoney = Money.Uah(500);

        // Act
        var result = money.Add(otherMoney);

        // Assert
        result.Amount.Should().Be(1500);
    }

    [Fact]
    public void ApplyPercentage_PositivePercentage_IncreasesAmount()
    {
        // Arrange
        var money = Money.Uah(1000);

        // Act
        var result = money.ApplyPercentage(15);

        // Assert
        result.Amount.Should().Be(1150);
    }

    [Fact]
    public void ApplyPercentage_NegativePercentage_DecreasesAmount()
    {
        // Arrange
        var money = Money.Uah(1000);

        // Act
        var result = money.ApplyPercentage(-20);

        // Assert
        result.Amount.Should().Be(800);
    }

    [Fact]
    public void Multiply_PositiveFactor_ReturnsScaledAmount()
    {
        // Arrange
        var money = Money.Uah(1500);

        // Act
        var result = money.Multiply(2);

        // Assert
        result.Amount.Should().Be(3000);
    }

    [Fact]
    public void Multiply_NegativeFactor_ThrowsArgumentException()
    {
        // Arrange
        var act = () => Money.Uah(1000).Multiply(-1);

        // Act & Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Equals_SameAmountAndCurrency_ReturnsTrue()
    {
        // Arrange
        var money = Money.Uah(1000);
        var otherMoney = Money.Uah(1000);

        // Act
        var result = money.Equals(otherMoney);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Equals_DifferentAmount_ReturnsFalse()
    {
        // Arrange
        var money = Money.Uah(1000);
        var otherMoney = Money.Uah(2000);

        // Act
        var result = money.Equals(otherMoney);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Zero_ReturnsAmountOfZero()
    {
        // Act
        var result = Money.Zero();

        // Assert
        result.Amount.Should().Be(0);
    }
}