using ConferenceBooking.Domain.Entities;

namespace ConferenceBooking.Application.Rooms.Dtos;

public record RoomDto(
    Guid Id,
    string Name,
    int Capacity,
    decimal BaseHourlyRate,
    bool IsActive,
    List<ServiceDto> Services)
{
    public static RoomDto FromEntity(Room room, IEnumerable<Service> services) =>
        new(
            room.Id,
            room.Name,
            room.Capacity,
            room.BaseHourlyRate.Amount,
            room.IsActive,
            services.Select(s => new ServiceDto(s.Id, s.Name, s.Price.Amount)).ToList());
}

public record ServiceDto(Guid Id, string Name, decimal Price);