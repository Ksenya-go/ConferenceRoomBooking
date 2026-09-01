using Mediator;


namespace ConferenceBooking.Application.Services.Commands;

public record CreateServiceCommand(string Name, decimal Price) : IRequest<Guid>;
