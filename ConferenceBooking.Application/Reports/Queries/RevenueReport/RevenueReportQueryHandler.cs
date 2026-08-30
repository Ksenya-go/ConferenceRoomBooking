using ConferenceBooking.Application.Common.Interfaces;
using ConferenceBooking.Application.Reports.Dtos;
using ConferenceBooking.Domain.Enums;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace ConferenceBooking.Application.Reports.Queries.RevenueReport;

public class RevenueReportQueryHandler : IRequestHandler<RevenueReportQuery, RevenueReportDto>
{
    private readonly IApplicationDbContext _context;

    public RevenueReportQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<RevenueReportDto> Handle(
        RevenueReportQuery request, CancellationToken cancellationToken)
    {
        // Беруться тільки підтверджені бронювання за вказаний період
        // (скасовані в дохід і статистику не рахуються)
        var bookings = await _context.Bookings
            .Where(b => b.Status == BookingStatus.Confirmed)
            .Where(b => b.TimeRange.Start >= request.PeriodStart && b.TimeRange.End <= request.PeriodEnd)
            .ToListAsync(cancellationToken);

        // Отримання Id залів, які використовувалися в цих бронюваннях
        var roomIds = bookings.Select(b => b.RoomId).Distinct().ToList();

        // Завантаження інформації про зали, щоб отримати їхні назви для звіту
        var rooms = await _context.Rooms
            .Where(r => roomIds.Contains(r.Id))
            .ToDictionaryAsync(r => r.Id, cancellationToken);

        // Групування бронювань за залом і для кожного залу розрахунок загального доходу
        // та кількості бронювань
        var byRoom = bookings
            .GroupBy(b => b.RoomId)
            .Select(g => new RevenueByRoomDto(
                g.Key,
                rooms.TryGetValue(g.Key, out var room) ? room.Name : "Невідомий зал",
                g.Sum(b => b.TotalPrice.Amount),
                g.Count()))
            .OrderByDescending(r => r.Revenue)
            .ToList();

        return new RevenueReportDto(
            request.PeriodStart,
            request.PeriodEnd,
            bookings.Sum(b => b.TotalPrice.Amount),
            bookings.Count,
            byRoom);
    }
}