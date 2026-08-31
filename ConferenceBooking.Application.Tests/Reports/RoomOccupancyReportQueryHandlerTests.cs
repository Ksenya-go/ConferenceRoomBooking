using ConferenceBooking.Application.Reports.Queries.RoomOccupancyReport;
using ConferenceBooking.Domain.Entities;
using ConferenceBooking.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ConferenceBooking.Application.Tests.Reports;

public class RoomOccupancyReportQueryHandlerTests : IDisposable
{
    private readonly TestDbContext _context;
    private readonly RoomOccupancyReportQueryHandler _handler;

    public RoomOccupancyReportQueryHandlerTests()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        _context = new TestDbContext(options);
        _handler = new RoomOccupancyReportQueryHandler(_context);
    }

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task Handle_RoomWithBookings_ReturnsCorrectBookingsCount()
    {
        // Arrange
        var room = Room.Create("Зал А",50,Money.Uah(2000));

        await _context.Rooms.AddAsync(room);

        var booking = Booking.Create(room.Id,TimeRange.Create(new DateTime(2026, 9, 1, 10, 0, 0),
            new DateTime(2026, 9, 1, 12, 0, 0)),Enumerable.Empty<Guid>(),Money.Uah(3000));

        await _context.Bookings.AddAsync(booking);
        await _context.SaveChangesAsync();

        var query = new RoomOccupancyReportQuery(new DateTime(2026, 9, 1),new DateTime(2026, 9, 2));

        // Act
        var result = await _handler.Handle(query,CancellationToken.None);

        // Assert
        var roomReport = result.Single(r => r.RoomId == room.Id);

        roomReport.TotalBookings.Should().Be(1);
        roomReport.TotalBookedHours.Should().Be(2);
    }

    [Fact]
    public async Task Handle_RoomWithoutBookings_ReturnsZeroOccupancy()
    {
        // Arrange
        var room = Room.Create("Зал B",100,Money.Uah(3500));

        await _context.Rooms.AddAsync(room);
        await _context.SaveChangesAsync();

        var query = new RoomOccupancyReportQuery(new DateTime(2026, 9, 1),new DateTime(2026, 9, 2));

        // Act
        var result = await _handler.Handle(query,CancellationToken.None);
         
        // Assert
        var roomReport = result.Single(r => r.RoomId == room.Id);

        roomReport.TotalBookings.Should().Be(0);
        roomReport.OccupancyRatePercent.Should().Be(0);
    }

    [Fact]
    public async Task Handle_InactiveRoom_ExcludedFromReport()
    {
        // Arrange
        var room = Room.Create("Зал C",30,Money.Uah(1500));

        room.Deactivate();

        await _context.Rooms.AddAsync(room);
        await _context.SaveChangesAsync();

        var query = new RoomOccupancyReportQuery(new DateTime(2026, 9, 1),new DateTime(2026, 9, 2));

        // Act
        var result = await _handler.Handle(query,CancellationToken.None);

        // Assert
        result.Should().NotContain(r => r.RoomId == room.Id);
    }
}