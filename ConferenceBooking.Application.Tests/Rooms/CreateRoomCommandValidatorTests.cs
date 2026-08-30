using ConferenceBooking.Application.Rooms.Commands;
using ConferenceBooking.Application.Rooms.Commands.CreateRoom;
using FluentValidation.TestHelper;

namespace ConferenceBooking.Application.Tests.Rooms;

public class CreateRoomCommandValidatorTests
{
    private readonly CreateRoomCommandValidator _validator = new();

    [Fact]
    public void Validate_EmptyName_HasValidationError()
    {
        // Arrange
        var command = new CreateRoomCommand("",50,2000);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_NameExceeds200Characters_HasValidationError()
    {
        // Arrange
        var longName = new string('А', 201);

        var command = new CreateRoomCommand(longName,50,2000);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_ZeroCapacity_HasValidationError()
    {
        // Arrange
        var command = new CreateRoomCommand("Зал А",0,2000);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Capacity);
    }

    [Fact]
    public void Validate_NegativeCapacity_HasValidationError()
    {
        // Arrange
        var command = new CreateRoomCommand("Зал А", -5, 2000);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Capacity);
    }

    [Fact]
    public void Validate_CapacityExceedsMaximum_HasValidationError()
    {
        // Arrange
        var command = new CreateRoomCommand("Зал А", 10001, 2000);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Capacity);
    }

    [Fact]
    public void Validate_ZeroBaseHourlyRate_HasValidationError()
    {
        // Arrange
        var command = new CreateRoomCommand("Зал А", 50, 0);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BaseHourlyRate);
    }

    [Fact]
    public void Validate_NegativeBaseHourlyRate_HasValidationError()
    {
        // Arrange
        var command = new CreateRoomCommand("Зал А", 50, -100);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BaseHourlyRate);
    }

    [Fact]
    public void Validate_ValidCommand_HasNoValidationErrors()
    {
        // Arrange
        var command = new CreateRoomCommand("Зал А",50,2000);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}