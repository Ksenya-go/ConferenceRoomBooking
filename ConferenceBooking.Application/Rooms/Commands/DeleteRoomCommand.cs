using Mediator;

namespace ConferenceBooking.Application.Rooms.Commands.DeleteRoom;

public record DeleteRoomCommand(Guid RoomId) : IRequest<Unit>;