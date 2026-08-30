using ConferenceBooking.Application.Common.ErrorMessages;
using FluentValidation;

namespace ConferenceBooking.Application.Rooms.Commands.CreateRoom;

public class CreateRoomCommandValidator : AbstractValidator<CreateRoomCommand>
{
    public CreateRoomCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(RoomErrorMessages.NameRequired)
            .MaximumLength(200).WithMessage(RoomErrorMessages.NameTooLong);

        RuleFor(x => x.Capacity)
            .GreaterThan(0).WithMessage(RoomErrorMessages.CapacityMustBePositive)
            .LessThanOrEqualTo(10000);

        RuleFor(x => x.BaseHourlyRate)
            .GreaterThan(0).WithMessage(RoomErrorMessages.BaseHourlyRateMustBePositive);
    }
}