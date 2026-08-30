using ConferenceBooking.Domain.Entities;
using ConferenceBooking.Domain.ValueObjects;
using FluentAssertions;


namespace ConferenceBooking.Domain.Tests.Entities;

public class RoomTests
{
    [Fact]
    public void Create_ValidData_ReturnsActiveRoom()
    {
        // Arrange
        var name = "Зал А";
        var capacity = 50;
        var hourlyRate = Money.Uah(2000);

        // Act
        var room = Room.Create(name, capacity, hourlyRate);

        // Assert
        room.Id.Should().NotBeEmpty();
        room.Name.Should().Be(name);
        room.Capacity.Should().Be(capacity);
        room.BaseHourlyRate.Amount.Should().Be(2000);
        room.IsActive.Should().BeTrue();
        room.Services.Should().BeEmpty();
    }

    [Fact]
    public void Create_NameWithExtraSpaces_TrimsName()
    {
        // Arrange
        var name = "Зал B";

        // Act
        var room = Room.Create(name, 100, Money.Uah(3500));

        // Assert
        room.Name.Should().Be("Зал B");
    }

    [Fact]
    public void Create_EmptyName_ThrowsArgumentException()
    {
        // Arrange
        var act = () => Room.Create("",50,Money.Uah(2000));

        // Act & Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WhitespaceOnlyName_ThrowsArgumentException()
    {
        // Arrange
        var act = () => Room.Create(" ",50,Money.Uah(2000));

        // Act & Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_ZeroCapacity_ThrowsArgumentException()
    {
        // Arrange
        var act = () => Room.Create("Зал А",0,Money.Uah(2000));

        // Act & Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_NegativeCapacity_ThrowsArgumentException()
    {
        // Arrange
        var act = () => Room.Create("Зал А",-10,Money.Uah(2000));

        // Act & Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void UpdateDetails_ValidData_UpdatesAllFields()
    {
        // Arrange
        var room = Room.Create("Зал А",50,Money.Uah(2000));

        var newName = "Зал А (оновлений)";
        var newCapacity = 60;
        var newHourlyRate = Money.Uah(2500);

        // Act
        room.UpdateDetails(newName,newCapacity,newHourlyRate);

        // Assert
        room.Name.Should().Be(newName);
        room.Capacity.Should().Be(newCapacity);
        room.BaseHourlyRate.Amount.Should().Be(2500);
    }

    [Fact]
    public void UpdateDetails_EmptyName_ThrowsArgumentException()
    {
        // Arrange
        var room = Room.Create("Зал А",50,Money.Uah(2000));

        var act = () => room.UpdateDetails("",60,Money.Uah(2500));

        // Act & Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void AddService_NewService_AddsToServicesList()
    {
        // Arrange
        var room = Room.Create("Зал А",50,Money.Uah(2000));

        var service = Service.Create("Wi-Fi",Money.Uah(300));

        // Act
        room.AddService(service);

        // Assert
        room.Services.Should().HaveCount(1);
        room.Services.Should().Contain(rs => rs.ServiceId == service.Id);
    }

    [Fact]
    public void AddService_SameServiceTwice_ThrowsInvalidOperationException()
    {
        // Arrange
        var room = Room.Create("Зал А",50,Money.Uah(2000));

        var service = Service.Create("Wi-Fi",Money.Uah(300));

        room.AddService(service);

        var act = () => room.AddService(service);

        // Act & Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void AddService_MultipleDifferentServices_AddsAllOfThem()
    {
        // Arrange
        var room = Room.Create("Зал B",100,Money.Uah(3500));

        var wifi = Service.Create("Wi-Fi",Money.Uah(300));

        var projector = Service.Create("Проєктор",Money.Uah(500));

        var sound = Service.Create("Звук",Money.Uah(700));

        // Act
        room.AddService(wifi);
        room.AddService(projector);
        room.AddService(sound);

        // Assert
        room.Services.Should().HaveCount(3);
    }

    [Fact]
    public void RemoveService_LinkedService_RemovesFromServicesList()
    {
        // Arrange
        var room = Room.Create("Зал А",50,Money.Uah(2000));

        var service = Service.Create("Wi-Fi",Money.Uah(300));
       
        room.AddService(service);

        // Act
        room.RemoveService(service.Id);

        // Assert
        room.Services.Should().BeEmpty();
    }

    [Fact]
    public void RemoveService_NotLinkedService_ThrowsInvalidOperationException()
    {
        // Arrange
        var room = Room.Create("Зал А",50,Money.Uah(2000));

        var serviceId = Guid.NewGuid();
        var act = () => room.RemoveService(serviceId);

        // Act & Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Theory]
    [InlineData(50, 30, true)]
    [InlineData(50, 50, true)]
    [InlineData(50, 80, false)]
    public void SupportsCapacity_VariousRequiredCapacities_ReturnsExpectedResult(
        int roomCapacity,
        int requiredCapacity,
        bool expected)
    {
        // Arrange
        var room = Room.Create("Зал",roomCapacity,Money.Uah(1000));

        // Act
        var result = room.SupportsCapacity(requiredCapacity);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void Deactivate_ActiveRoom_SetsIsActiveToFalse()
    {
        // Arrange
        var room = Room.Create("Зал А",50,Money.Uah(2000));

        // Act
        room.Deactivate();

        // Assert
        room.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Activate_InactiveRoom_SetsIsActiveToTrue()
    {
        // Arrange
        var room = Room.Create("Зал А",50,Money.Uah(2000));

        room.Deactivate();

        // Act
        room.Activate();

        // Assert
        room.IsActive.Should().BeTrue();
    }
}