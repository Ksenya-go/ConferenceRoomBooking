using ConferenceBooking.Application.Rooms.Queries.SearchAvailableRooms;
using ConferenceBooking.Domain.Entities;
using ConferenceBooking.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ConferenceBooking.Application.Tests.Rooms;

public class SearchAvailableRoomsQueryHandlerTests : IDisposable
{
    private readonly TestDbContext _context;
    private readonly SearchAvailableRoomsQueryHandler _handler;

    public SearchAvailableRoomsQueryHandlerTests()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new TestDbContext(options);
        _handler = new SearchAvailableRoomsQueryHandler(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task Handle_RoomWithSufficientCapacity_ReturnsRoom()
    {
        // Arrange
        var room = Room.Create("Зал B",100,Money.Uah(3500));

        await _context.Rooms.AddAsync(room);
        await _context.SaveChangesAsync();

        var query = new SearchAvailableRoomsQuery(new DateTime(2026, 9, 1, 10, 0, 0),
            new DateTime(2026, 9, 1, 12, 0, 0),
            80);

        // Act
        var result = await _handler.Handle(query,CancellationToken.None);

        // Assert
        result.Should().ContainSingle(r => r.Id == room.Id);
    }

    [Fact]
    public async Task Handle_RoomWithInsufficientCapacity_ExcludesRoom()
    {
        // Arrange
        var room = Room.Create("Зал C",30,Money.Uah(1500));

        await _context.Rooms.AddAsync(room);
        await _context.SaveChangesAsync();

        var query = new SearchAvailableRoomsQuery(new DateTime(2026, 9, 1, 10, 0, 0),
            new DateTime(2026, 9, 1, 12, 0, 0),
            80);

        // Act
        var result = await _handler.Handle(query,CancellationToken.None);

        // Assert
        result.Should().NotContain(r => r.Id == room.Id);
    }

    [Fact]
    public async Task Handle_InactiveRoom_ExcludesRoom()
    {
        // Arrange
        var room = Room.Create("Зал А",50,Money.Uah(2000));

        room.Deactivate();

        await _context.Rooms.AddAsync(room);
        await _context.SaveChangesAsync();

        var query = new SearchAvailableRoomsQuery(new DateTime(2026, 9, 1, 10, 0, 0),
            new DateTime(2026, 9, 1, 12, 0, 0),10);

        // Act
        var result = await _handler.Handle(query,CancellationToken.None);

        // Assert
        result.Should().NotContain(r => r.Id == room.Id);
    }

    [Fact]
    public async Task Handle_RoomWithOverlappingBooking_ExcludesRoom()
    {
        // Arrange
        var room = Room.Create("Зал А",50,Money.Uah(2000));

        await _context.Rooms.AddAsync(room);

        var timeRange = TimeRange.Create(new DateTime(2026, 9, 1, 10, 0, 0),
            new DateTime(2026, 9, 1, 12, 0, 0));

        var booking = Booking.Create(room.Id,timeRange,Enumerable.Empty<Guid>(),Money.Uah(3000));

        await _context.Bookings.AddAsync(booking);
        await _context.SaveChangesAsync();

        var query = new SearchAvailableRoomsQuery(new DateTime(2026, 9, 1, 11, 0, 0),
            new DateTime(2026, 9, 1, 13, 0, 0),
            10);

        // Act
        var result = await _handler.Handle(query,CancellationToken.None);
   
        // Assert
        result.Should().NotContain(r => r.Id == room.Id);
    }

    [Fact]
    public async Task Handle_RoomFreeAtRequestedTime_IncludesRoom()
    {
        // Arrange
        var room = Room.Create("Зал А",50,Money.Uah(2000));

        await _context.Rooms.AddAsync(room);

        var existingTimeRange = TimeRange.Create(new DateTime(2026, 9, 1, 10, 0, 0),
            new DateTime(2026, 9, 1, 12, 0, 0));

        var booking = Booking.Create(room.Id,existingTimeRange,Enumerable.Empty<Guid>(),
            Money.Uah(3000));

        await _context.Bookings.AddAsync(booking);
        await _context.SaveChangesAsync();

        var query = new SearchAvailableRoomsQuery(new DateTime(2026, 9, 1, 14, 0, 0),
            new DateTime(2026, 9, 1, 16, 0, 0),10);

        // Act
        var result = await _handler.Handle(query,CancellationToken.None);

        // Assert
        result.Should().ContainSingle(r => r.Id == room.Id);
    }
}