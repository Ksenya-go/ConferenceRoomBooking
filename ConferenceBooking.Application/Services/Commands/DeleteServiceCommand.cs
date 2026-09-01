using Mediator;

namespace ConferenceBooking.Application.Services.Commands;

public record DeleteServiceCommand(Guid ServiceId) : IRequest<Unit>;