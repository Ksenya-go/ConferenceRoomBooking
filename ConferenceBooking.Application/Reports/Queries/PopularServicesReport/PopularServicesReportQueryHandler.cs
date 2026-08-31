using ConferenceBooking.Application.Common.Interfaces;
using ConferenceBooking.Application.Reports.Common;
using ConferenceBooking.Application.Reports.Dtos;
using Mediator;

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
        var bookings = await ReportQueryHelpers.GetConfirmedBookingsInPeriodAsync(
            _context, request.PeriodStart, request.PeriodEnd, cancellationToken);

        // Групування за знімком послуги на момент бронювання,щоб звіт використовував фактичну ціну
        var result = bookings
            .SelectMany(b => b.Services)
            .GroupBy(bs => new { bs.ServiceId, bs.ServiceName })
            .Select(g => new PopularServicesReportDto(
                g.Key.ServiceId,
                g.Key.ServiceName,
                g.Count(),
                g.Sum(bs => bs.PriceAtBooking.Amount)))
            .OrderByDescending(s => s.TimesOrdered)
            .ToList();

        return result;
    }
}