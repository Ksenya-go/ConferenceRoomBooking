using Mediator;

namespace ConferenceBooking.Application.Rooms.Commands;

public record CreateRoomCommand(
    string Name,
    int Capacity,
    decimal BaseHourlyRate,
    List<Guid>? ServiceIds = null) : IRequest<Guid>;