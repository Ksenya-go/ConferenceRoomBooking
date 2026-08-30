using ConferenceBooking.Application.Common.ErrorMessages;
using FluentValidation;

namespace ConferenceBooking.Application.Rooms.Queries.SearchAvailableRooms;

public class SearchAvailableRoomsQueryValidator : AbstractValidator<SearchAvailableRoomsQuery>
{
    public SearchAvailableRoomsQueryValidator()
    {
        RuleFor(x => x.End)
            .GreaterThan(x => x.Start)
            .WithMessage(RoomErrorMessages.EndMustBeAfterStartForSearch);

        RuleFor(x => x.RequiredCapacity)
            .GreaterThan(0)
            .WithMessage(RoomErrorMessages.CapacityMustBePositive);
    }
}