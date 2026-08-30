using ConferenceBooking.Application.Reports.Dtos;
using Mediator;

namespace ConferenceBooking.Application.Reports.Queries.RoomOccupancyReport;


public record RoomOccupancyReportQuery(DateTime PeriodStart, DateTime PeriodEnd)
    : IRequest<List<RoomOccupancyReportDto>>;