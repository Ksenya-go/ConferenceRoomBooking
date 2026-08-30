using Mediator;

namespace ConferenceBooking.Application.Rooms.Commands.AddServiceToRoom;

public record AddServiceToRoomCommand(Guid RoomId, Guid ServiceId) : IRequest<Unit>;