using ConferenceBooking.Application.Common.ErrorMessages;
using FluentValidation;

namespace ConferenceBooking.Application.Rooms.Commands.AddServiceToRoom;

public class AddServiceToRoomCommandValidator : AbstractValidator<AddServiceToRoomCommand>
{
    public AddServiceToRoomCommandValidator()
    {
        RuleFor(x => x.RoomId)
            .NotEmpty().WithMessage(RoomErrorMessages.RoomIdRequired);

        RuleFor(x => x.ServiceId)
            .NotEmpty().WithMessage(RoomErrorMessages.ServiceIdRequired);
    }
}