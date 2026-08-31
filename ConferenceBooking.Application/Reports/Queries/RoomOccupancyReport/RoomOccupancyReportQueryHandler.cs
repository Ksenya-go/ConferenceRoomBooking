using ConferenceBooking.Application.Common.Interfaces;
using ConferenceBooking.Application.Reports.Common;
using ConferenceBooking.Application.Reports.Dtos;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace ConferenceBooking.Application.Reports.Queries.RoomOccupancyReport;

public class RoomOccupancyReportQueryHandler
    : IRequestHandler<RoomOccupancyReportQuery, List<RoomOccupancyReportDto>>
{
    // Зал доступний для бронювання 17 годин на добу: з 06:00 до 23:00.
    private const decimal AvailableHoursPerDay = 17m;

    private readonly IApplicationDbContext _context;

    public RoomOccupancyReportQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<List<RoomOccupancyReportDto>> Handle(
        RoomOccupancyReportQuery request, CancellationToken cancellationToken)
    {
        // Отримання всіх активних залів, для яких потрібно розрахувати завантаженість
        var rooms = await _context.Rooms
            .Where(r => r.IsActive)
            .ToListAsync(cancellationToken);

        // Отримання всіх бронювань за вказаний період
        var bookings = await ReportQueryHelpers.GetBookingsInPeriodAsync(
                       _context, request.PeriodStart, request.PeriodEnd, cancellationToken);

        // Розрахунок загальної кількості днів у періоді
        var totalDays = Math.Max(1, (request.PeriodEnd.Date - request.PeriodStart.Date).Days);
        
        // Розрахунок загальної кількості доступних годин для бронювання
        var availableHours = totalDays * AvailableHoursPerDay;

        var result = new List<RoomOccupancyReportDto>();

        foreach (var room in rooms)
        {
            // Отримання всіх бронювань для конкретного залу
            var roomBookings = bookings.Where(b => b.RoomId == room.Id).ToList();
            // Розрахунок загальної кількості заброньованих годин для конкретного залу
            var bookedHours = roomBookings.Sum(b => (decimal)b.TimeRange.Duration.TotalHours);
            
            // Розрахунок відсотка завантаженості залу
            var occupancyRate = availableHours > 0
                ? Math.Round(bookedHours / availableHours * 100, 2)
                : 0;

            result.Add(new RoomOccupancyReportDto(
                room.Id,
                room.Name,
                roomBookings.Count,
                Math.Round(bookedHours, 2),
                occupancyRate));
        }

        return result.OrderByDescending
            (r => r.OccupancyRatePercent).ToList();
    }
}