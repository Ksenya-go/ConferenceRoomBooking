using ConferenceBooking.Application.Rooms.Dtos;
using Mediator;

namespace ConferenceBooking.Application.Rooms.Queries.GetRoomById;

public record GetRoomByIdQuery(Guid RoomId) : IRequest<RoomDto>;