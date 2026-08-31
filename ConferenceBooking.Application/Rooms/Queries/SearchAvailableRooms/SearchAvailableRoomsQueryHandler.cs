using ConferenceBooking.Application.Common.Interfaces;
using ConferenceBooking.Application.Rooms.Dtos;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace ConferenceBooking.Application.Rooms.Queries.SearchAvailableRooms;

public class SearchAvailableRoomsQueryHandler
    : IRequestHandler<SearchAvailableRoomsQuery, List<RoomDto>>
{
    private readonly IApplicationDbContext _context;

    public SearchAvailableRoomsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<List<RoomDto>> Handle(
        SearchAvailableRoomsQuery request, CancellationToken cancellationToken)
    {
        // Отримання всіх активних залів, які відповідають вимогам щодо місткості
        var candidateRooms = await _context.Rooms
            .Include(r => r.Services)
            .Where(r => r.IsActive && r.Capacity >= request.RequiredCapacity)
            .ToListAsync(cancellationToken);

        if (candidateRooms.Count == 0)
        {
            return new List<RoomDto>();
        }
        // Отримання ID залів
        var candidateIds = candidateRooms.Select(r => r.Id).ToList();

        // Перевірка наявності конфліктуючих бронювань у вказаному часовому проміжку
        var conflictingRoomIds = await _context.Bookings
            .Where(b => candidateIds.Contains(b.RoomId))
            .Where(b => b.TimeRange.Start < request.End && request.Start < b.TimeRange.End)
            .Select(b => b.RoomId)
            .Distinct()
            .ToListAsync(cancellationToken);

        // Фільтрація доступних залів, виключаючи ті, що мають конфліктуючі бронювання
        var availableRooms = candidateRooms
            .Where(r => !conflictingRoomIds.Contains(r.Id))
            .ToList();

        // Отримання ID послуг, які відповідають доступним залам
        var serviceIds = availableRooms.SelectMany(r => r.Services.Select(s => s.ServiceId)).Distinct().
            ToList();
        // Отримання всіх послуг, які відповідають доступним залам
        var services = await _context.Services
            .Where(s => serviceIds.Contains(s.Id))
            .ToDictionaryAsync(s => s.Id, cancellationToken);

        return availableRooms
        .Select(r => RoomDto.FromEntity(r,r.Services
             .Where(rs => services.ContainsKey(rs.ServiceId))
             .Select(rs => services[rs.ServiceId])))
        .ToList();
    }
}