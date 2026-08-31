using ConferenceBooking.Application.Reports.Queries.PopularServicesReport;
using ConferenceBooking.Domain.Entities;
using ConferenceBooking.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ConferenceBooking.Application.Tests.Reports;

public class PopularServicesReportQueryHandlerTests : IDisposable
{
    private readonly TestDbContext _context;
    private readonly PopularServicesReportQueryHandler _handler;

    public PopularServicesReportQueryHandlerTests()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        _context = new TestDbContext(options);
        _handler = new PopularServicesReportQueryHandler(_context);
    }

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task Handle_ServiceUsedInMultipleBookings_CountsCorrectly()
    {
        // Arrange
        var room = Room.Create("Зал А",50,Money.Uah(2000));

        var wifi = Service.Create("Wi-Fi",Money.Uah(300));

        await _context.Rooms.AddAsync(room);
        await _context.Services.AddAsync(wifi);

        var booking1 = Booking.Create(
            room.Id,
            TimeRange.Create(new DateTime(2026, 9, 1, 10, 0, 0),new DateTime(2026, 9, 1, 12, 0, 0)),
            new[] { wifi.Id },Money.Uah(3300));

        var booking2 = Booking.Create(
            room.Id,
            TimeRange.Create(new DateTime(2026, 9, 2, 10, 0, 0),new DateTime(2026, 9, 2, 12, 0, 0)),
            new[] { wifi.Id },Money.Uah(3300));

        await _context.Bookings.AddRangeAsync(booking1, booking2);
        await _context.SaveChangesAsync();

        var query = new PopularServicesReportQuery(new DateTime(2026, 9, 1),new DateTime(2026, 9, 10));

        // Act
        var result = await _handler.Handle(query,CancellationToken.None);

        // Assert
        result.Should().ContainSingle(s => s.ServiceId == wifi.Id && s.TimesOrdered == 2);
    }

    [Fact]
    public async Task Handle_NoBookingsWithServices_ReturnsEmptyList()
    {
        // Arrange
        var query = new PopularServicesReportQuery(new DateTime(2026, 9, 1),new DateTime(2026, 9, 10));

        // Act
        var result = await _handler.Handle(query,CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }
}