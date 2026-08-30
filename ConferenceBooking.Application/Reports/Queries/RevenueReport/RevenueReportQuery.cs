using ConferenceBooking.Application.Reports.Dtos;
using Mediator;

namespace ConferenceBooking.Application.Reports.Queries.RevenueReport;

public record RevenueReportQuery(DateTime PeriodStart, DateTime PeriodEnd)
    : IRequest<RevenueReportDto>;