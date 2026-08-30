using ConferenceBooking.Application.Common.ErrorMessages;
using FluentValidation;

namespace ConferenceBooking.Application.Rooms.Commands.UpdateRoom;

public class UpdateRoomCommandValidator : AbstractValidator<UpdateRoomCommand>
{
    public UpdateRoomCommandValidator()
    {
        RuleFor(x => x.RoomId)
            .NotEmpty().WithMessage(RoomErrorMessages.RoomIdRequired);

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(RoomErrorMessages.NameRequired)
            .MaximumLength(200).WithMessage(RoomErrorMessages.NameTooLong);

        RuleFor(x => x.Capacity)
            .GreaterThan(0).WithMessage(RoomErrorMessages.CapacityMustBePositive);

        RuleFor(x => x.BaseHourlyRate)
            .GreaterThan(0).WithMessage(RoomErrorMessages.BaseHourlyRateMustBePositive);
    }
}