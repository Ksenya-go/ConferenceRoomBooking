using ConferenceBooking.Application.Common.Interfaces;
using ConferenceBooking.Domain.Entities;
using ConferenceBooking.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ConferenceBooking.Application.Reports.Common;

public static class ReportQueryHelpers
{
    public static Task<List<Booking>> GetBookingsInPeriodAsync(
        IApplicationDbContext context, DateTime periodStart, DateTime periodEnd, CancellationToken ct)
    {
        return context.Bookings
            .Where(b => b.TimeRange.Start >= periodStart && b.TimeRange.End <= periodEnd)
            .ToListAsync(ct);
    }

    public static Task<List<Booking>> GetConfirmedBookingsInPeriodAsync(
        IApplicationDbContext context, DateTime periodStart, DateTime periodEnd, CancellationToken ct)
    {
        return context.Bookings
            .Where(b => b.Status == BookingStatus.Confirmed)
            .Where(b => b.TimeRange.Start >= periodStart && b.TimeRange.End <= periodEnd)
            .ToListAsync(ct);
    }
        
}