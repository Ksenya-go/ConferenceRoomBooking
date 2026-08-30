using ConferenceBooking.Application.Common.ErrorMessages;
using FluentValidation;

namespace ConferenceBooking.Application.Bookings.Commands;

public class CreateBookingCommandValidator : AbstractValidator<CreateBookingCommand>
{
    public CreateBookingCommandValidator()
    {
        RuleFor(x => x.RoomId)
            .NotEmpty()
            .WithMessage(BookingErrorMessages.RoomIdRequired);

        RuleFor(x => x.End)
            .GreaterThan(x => x.Start)
            .WithMessage(BookingErrorMessages.EndMustBeAfterStart);

        RuleFor(x => x.Start)
            .GreaterThan(_ => DateTime.UtcNow.AddMinutes(-5))
            .WithMessage(BookingErrorMessages.CannotBookInThePast);

        RuleForEach(x => x.SelectedServiceIds)
            .NotEmpty()
            .WithMessage(BookingErrorMessages.ServiceIdRequired);
    }
}