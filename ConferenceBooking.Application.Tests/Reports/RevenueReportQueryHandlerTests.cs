using ConferenceBooking.Application.Reports.Queries.RevenueReport;
using ConferenceBooking.Domain.Entities;
using ConferenceBooking.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ConferenceBooking.Application.Tests.Reports;

public class RevenueReportQueryHandlerTests : IDisposable
{
    private readonly TestDbContext _context;
    private readonly RevenueReportQueryHandler _handler;

    public RevenueReportQueryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        _context = new TestDbContext(options);
        _handler = new RevenueReportQueryHandler(_context);
    }

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task Handle_ConfirmedBookingsInPeriod_SumsTotalRevenue()
    {
        // Arrange: два бронювання одного залу в межах періоду
        var room = Room.Create("Зал А", 50, Money.Uah(2000));
        await _context.Rooms.AddAsync(room);

        var booking1 = Booking.Create(room.Id,TimeRange.Create(new DateTime(2026, 9, 1, 10, 0, 0), 
            new DateTime(2026, 9, 1, 12, 0, 0)),Enumerable.Empty<Guid>(),Money.Uah(3000));

        var booking2 = Booking.Create(room.Id,TimeRange.Create(new DateTime(2026, 9, 2, 10, 0, 0), 
            new DateTime(2026, 9, 2, 12, 0, 0)),Enumerable.Empty<Guid>(),Money.Uah(4000));

        await _context.Bookings.AddRangeAsync(booking1, booking2);
        await _context.SaveChangesAsync();

        var query = new RevenueReportQuery(new DateTime(2026, 9, 1), new DateTime(2026, 9, 10));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.TotalRevenue.Should().Be(7000);
        result.TotalBookings.Should().Be(2);
        result.ByRoom.Should().ContainSingle(r => r.RoomId == room.Id && r.Revenue == 7000);
    }

    [Fact]
    public async Task Handle_CancelledBooking_ExcludedFromRevenue()
    {
        // Arrange
        var room = Room.Create("Зал А", 50, Money.Uah(2000));
        await _context.Rooms.AddAsync(room);

        var booking = Booking.Create(room.Id,TimeRange.Create(new DateTime(2026, 9, 1, 10, 0, 0), 
            new DateTime(2026, 9, 1, 12, 0, 0)),Enumerable.Empty<Guid>(),Money.Uah(3000));
        booking.Cancel();

        await _context.Bookings.AddAsync(booking);
        await _context.SaveChangesAsync();

        var query = new RevenueReportQuery(new DateTime(2026, 9, 1), new DateTime(2026, 9, 10));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert: скасоване бронювання не рахується в дохід
        result.TotalRevenue.Should().Be(0);
        result.TotalBookings.Should().Be(0);
    }

    [Fact]
    public async Task Handle_BookingOutsidePeriod_ExcludedFromReport()
    {
        // Arrange: бронювання поза межами запитаного періоду
        var room = Room.Create("Зал А", 50, Money.Uah(2000));
        await _context.Rooms.AddAsync(room);

        var booking = Booking.Create(room.Id,TimeRange.Create(new DateTime(2026, 10, 1, 10, 0, 0), 
            new DateTime(2026, 10, 1, 12, 0, 0)),Enumerable.Empty<Guid>(),Money.Uah(3000));

        await _context.Bookings.AddAsync(booking);
        await _context.SaveChangesAsync();

        var query = new RevenueReportQuery(new DateTime(2026, 9, 1), new DateTime(2026, 9, 30));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.TotalRevenue.Should().Be(0);
    }
}