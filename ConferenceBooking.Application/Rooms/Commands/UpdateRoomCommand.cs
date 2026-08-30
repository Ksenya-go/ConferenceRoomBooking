using Mediator;

namespace ConferenceBooking.Application.Rooms.Commands;

public record UpdateRoomCommand(
    Guid RoomId,
    string Name,
    int Capacity,
    decimal BaseHourlyRate) : IRequest<Unit>;