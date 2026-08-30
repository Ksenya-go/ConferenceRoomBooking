using Mediator;

namespace ConferenceBooking.Application.Rooms.Commands;

public record CreateRoomCommand(
    string Name,
    int Capacity,
    decimal BaseHourlyRate) : IRequest<Guid>;