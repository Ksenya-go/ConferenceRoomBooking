using ConferenceBooking.Application.Common.Interfaces;
using ConferenceBooking.Application.Reports.Dtos;
using ConferenceBooking.Domain.Enums;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace ConferenceBooking.Application.Reports.Queries.PopularServicesReport;

public class PopularServicesReportQueryHandler
    : IRequestHandler<PopularServicesReportQuery, List<PopularServicesReportDto>>
{
    private readonly IApplicationDbContext _context;

    public PopularServicesReportQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<List<PopularServicesReportDto>> Handle(
        PopularServicesReportQuery request, CancellationToken cancellationToken)
    {
        // Беруться тільки підтверджені бронювання за вказаний період
        // (скасовані в дохід і статистику не рахуються)

        var bookings = await _context.Bookings
            .Where(b => b.Status == BookingStatus.Confirmed)
            .Where(b => b.TimeRange.Start >= request.PeriodStart && b.TimeRange.End <= request.PeriodEnd)
            .ToListAsync(cancellationToken);

        // Отримання Id послуг, які були вибрані в цих бронюваннях
        var allServiceIds = bookings.SelectMany(b => b.SelectedServiceIds).Distinct().ToList();

        // Завантаження послуг з бази, щоб отримати їх назви та актуальні ціни
        var services = await _context.Services
            .Where(s => allServiceIds.Contains(s.Id))
            .ToDictionaryAsync(s => s.Id, cancellationToken);

        // Групування послуг за Id та розрахунок,скільки разів кожна послуга була замовлена
        var result = bookings
            .SelectMany(b => b.SelectedServiceIds)
            .Where(services.ContainsKey)
            .GroupBy(id => id)
            .Select(g => new PopularServicesReportDto(
                g.Key,
                services[g.Key].Name,
                g.Count(),
                g.Count() * services[g.Key].Price.Amount))
            .OrderByDescending(s => s.TimesOrdered)
            .ToList();

        return result;
    }
}