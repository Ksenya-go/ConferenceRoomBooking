namespace ConferenceBooking.Application.Rooms.Dtos;

public record RoomDto(
    Guid Id,
    string Name,
    int Capacity,
    decimal BaseHourlyRate,
    bool IsActive,
    List<ServiceDto> Services);

public record ServiceDto(Guid Id, string Name, decimal Price);