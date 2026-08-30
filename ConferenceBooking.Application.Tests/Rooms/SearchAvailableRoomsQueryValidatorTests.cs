using ConferenceBooking.Application.Rooms.Queries.SearchAvailableRooms;
using FluentValidation.TestHelper;

namespace ConferenceBooking.Application.Tests.Rooms;

public class SearchAvailableRoomsQueryValidatorTests
{
    private readonly SearchAvailableRoomsQueryValidator _validator = new();

    [Fact]
    public void Validate_EndBeforeStart_HasValidationError()
    {
        // Arrange
        var start = DateTime.UtcNow.AddDays(1);
        var query = new SearchAvailableRoomsQuery(start,start.AddHours(-1),10);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.End);
    }

    [Fact]
    public void Validate_ZeroCapacity_HasValidationError()
    {
        // Arrange
        var start = DateTime.UtcNow.AddDays(1);
        var query = new SearchAvailableRoomsQuery(start,start.AddHours(2),0);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RequiredCapacity);
    }

    [Fact]
    public void Validate_ValidQuery_HasNoValidationErrors()
    {
        // Arrange
        var start = DateTime.UtcNow.AddDays(1);
        var query = new SearchAvailableRoomsQuery(start,start.AddHours(2),10);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}