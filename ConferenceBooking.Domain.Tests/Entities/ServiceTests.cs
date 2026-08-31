using ConferenceBooking.Domain.Entities;
using ConferenceBooking.Domain.ValueObjects;
using FluentAssertions;

namespace ConferenceBooking.Domain.Tests.Entities;

public class ServiceTests
{
    [Fact]
    public void Create_ValidData_ReturnsActiveService()
    {
        // Arrange
        var name = "Wi-Fi";
        var price = Money.Uah(300);

        // Act
        var service = Service.Create(name, price);

        // Assert
        service.Id.Should().NotBeEmpty();
        service.Name.Should().Be(name);
        service.Price.Amount.Should().Be(300);
        service.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Create_NameWithExtraSpaces_TrimsName()
    {
        // Arrange
        var name = " Проєктор ";

        // Act
        var service = Service.Create(name, Money.Uah(500));

        // Assert
        service.Name.Should().Be("Проєктор");
    }

    [Fact]
    public void Create_EmptyName_ThrowsArgumentException()
    {
        // Arrange
        var act = () => Service.Create("", Money.Uah(300));

        // Act & Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WhitespaceOnlyName_ThrowsArgumentException()
    {
        // Arrange
        var act = () => Service.Create("   ", Money.Uah(300));

        // Act & Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void UpdatePrice_NewPrice_ChangesPrice()
    {
        // Arrange
        var service = Service.Create("Wi-Fi", Money.Uah(300));
        var newPrice = Money.Uah(400);

        // Act
        service.UpdatePrice(newPrice);

        // Assert
        service.Price.Amount.Should().Be(400);
    }

    [Fact]
    public void Rename_ValidName_ChangesName()
    {
        // Arrange
        var service = Service.Create("Wi-Fi", Money.Uah(300));

        // Act
        service.Rename("Швидкий Wi-Fi");

        // Assert
        service.Name.Should().Be("Швидкий Wi-Fi");
    }

    [Fact]
    public void Rename_EmptyName_ThrowsArgumentException()
    {
        // Arrange
        var service = Service.Create("Wi-Fi", Money.Uah(300));

        // Act
        var act = () => service.Rename("");

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Deactivate_ActiveService_SetsIsActiveToFalse()
    {
        // Arrange
        var service = Service.Create("Wi-Fi", Money.Uah(300));

        // Act
        service.Deactivate();

        // Assert
        service.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Activate_InactiveService_SetsIsActiveToTrue()
    {
        // Arrange
        var service = Service.Create("Wi-Fi", Money.Uah(300));
        service.Deactivate();

        // Act
        service.Activate();

        // Assert
        service.IsActive.Should().BeTrue();
    }
}