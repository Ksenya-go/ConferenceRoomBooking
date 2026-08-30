using ConferenceBooking.Application.Bookings.Commands;
using FluentValidation.TestHelper;

namespace ConferenceBooking.Application.Tests.Bookings;

public class CreateBookingCommandValidatorTests
{
    private readonly CreateBookingCommandValidator _validator = new();

    [Fact]
    public void Validate_EmptyRoomId_HasValidationError()
    {
        // Arrange
        var command = new CreateBookingCommand(
            Guid.Empty,
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(1).AddHours(2),
            new List<Guid>());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RoomId);
    }

    [Fact]
    public void Validate_EndBeforeStart_HasValidationError()
    {
        // Arrange
        var start = DateTime.UtcNow.AddDays(1);

        var command = new CreateBookingCommand(
            Guid.NewGuid(),
            start,
            start.AddHours(-1),
            new List<Guid>());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.End);
    }

    [Fact]
    public void Validate_StartInThePast_HasValidationError()
    {
        // Arrange
        var start = DateTime.UtcNow.AddDays(-1);
        var end = start.AddHours(2);

        var command = new CreateBookingCommand(
            Guid.NewGuid(),
            start,
            end,
            new List<Guid>());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Start);
    }

    [Fact]
    public void Validate_StartFewMinutesInPastWithinTolerance_HasNoValidationError()
    {
        // Arrange
       
        var start = DateTime.UtcNow.AddMinutes(-2); //Початок бронювання знаходиться на 2
                                                    //хвилини в минулому
        var end = DateTime.UtcNow.AddHours(1);

        var command = new CreateBookingCommand(
            Guid.NewGuid(),
            start,
            end,
            new List<Guid>());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Start);
    }

    [Fact]
    public void Validate_ValidCommand_HasNoValidationErrors()
    {
        // Arrange
        var start = DateTime.UtcNow.AddDays(1);
        var end = start.AddHours(2);

        var command = new CreateBookingCommand(
            Guid.NewGuid(),
            start,
            end,
            new List<Guid>
            {
                Guid.NewGuid()
            });

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyServiceIdInList_HasValidationError()
    {
        // Arrange
        var start = DateTime.UtcNow.AddDays(1);
        var end = start.AddHours(2);

        var command = new CreateBookingCommand(
            Guid.NewGuid(),
            start,
            end,
            new List<Guid>
            {
                Guid.Empty
            });

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SelectedServiceIds);
    }
}