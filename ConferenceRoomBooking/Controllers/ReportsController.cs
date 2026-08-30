using ConferenceBooking.Application.Reports.Queries.PopularServicesReport;
using ConferenceBooking.Application.Reports.Queries.RevenueReport;
using ConferenceBooking.Application.Reports.Queries.RoomOccupancyReport;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceBooking.Api.Controllers;

[ApiController]
[Route("api/reports")]
public class ReportsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReportsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("occupancy")]
    public async Task<IActionResult> GetOccupancy(
        [FromQuery] DateTime periodStart, [FromQuery] DateTime periodEnd, CancellationToken ct)
    {
        var report = await _mediator.Send(new RoomOccupancyReportQuery(periodStart, periodEnd), ct);
        return Ok(report);
    }

    [HttpGet("revenue")]
    public async Task<IActionResult> GetRevenue(
        [FromQuery] DateTime periodStart, [FromQuery] DateTime periodEnd, CancellationToken ct)
    {
        var report = await _mediator.Send(new RevenueReportQuery(periodStart, periodEnd), ct);
        return Ok(report);
    }

    [HttpGet("popular-services")]
    public async Task<IActionResult> GetPopularServices(
        [FromQuery] DateTime periodStart, [FromQuery] DateTime periodEnd, CancellationToken ct)
    {
        var report = await _mediator.Send(new PopularServicesReportQuery(periodStart, periodEnd), ct);
        return Ok(report);
    }
}