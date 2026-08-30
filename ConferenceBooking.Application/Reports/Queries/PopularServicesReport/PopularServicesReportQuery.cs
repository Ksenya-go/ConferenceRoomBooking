using ConferenceBooking.Application.Reports.Dtos;
using Mediator;

namespace ConferenceBooking.Application.Reports.Queries.PopularServicesReport;

public record PopularServicesReportQuery(DateTime PeriodStart, DateTime PeriodEnd)
    : IRequest<List<PopularServicesReportDto>>;