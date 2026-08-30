using ConferenceBooking.Application.Rooms.Dtos;
using Mediator;

namespace ConferenceBooking.Application.Rooms.Queries.SearchAvailableRooms;

public record SearchAvailableRoomsQuery(
    DateTime Start,
    DateTime End,
    int RequiredCapacity) : IRequest<List<RoomDto>>;