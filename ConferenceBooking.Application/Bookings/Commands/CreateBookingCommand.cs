using Mediator;

namespace ConferenceBooking.Application.Bookings.Commands;

public record CreateBookingCommand(
    Guid RoomId,
    DateTime Start,
    DateTime End,
    List<Guid> SelectedServiceIds) : IRequest<CreateBookingResult>;

public record CreateBookingResult(Guid BookingId, decimal TotalPrice);