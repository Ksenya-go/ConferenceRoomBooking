using Mediator;


namespace ConferenceBooking.Application.Services.Commands;

public record UpdateServiceCommand(Guid ServiceId, string Name, decimal Price) : IRequest<Unit>;