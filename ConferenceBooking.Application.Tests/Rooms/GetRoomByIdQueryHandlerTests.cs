using ConferenceBooking.Application.Common.Exceptions;
using ConferenceBooking.Application.Rooms.Queries.GetRoomById;
using ConferenceBooking.Domain.Entities;
using ConferenceBooking.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ConferenceBooking.Application.Tests.Rooms;

public class GetRoomByIdQueryHandlerTests : IDisposable
{
    private readonly TestDbContext _context;
    private readonly GetRoomByIdQueryHandler _handler;

    public GetRoomByIdQueryHandlerTests()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new TestDbContext(options);
        _handler = new GetRoomByIdQueryHandler(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task Handle_ExistingRoomWithServices_ReturnsRoomDtoWithServices()
    {
        // Arrange
        var room = Room.Create("Зал C",30,Money.Uah(1500));

        var wifi = Service.Create("Wi-Fi",Money.Uah(300));

        room.AddService(wifi);

        await _context.Services.AddAsync(wifi);
        await _context.Rooms.AddAsync(room);
        await _context.SaveChangesAsync();

        var query = new GetRoomByIdQuery(room.Id);

        // Act
        var result = await _handler.Handle(query,CancellationToken.None);

        // Assert
        result.Id.Should().Be(room.Id);
        result.Name.Should().Be("Зал C");
        result.Services.Should().ContainSingle(s => s.Name == "Wi-Fi");
    }

    [Fact]
    public async Task Handle_NonExistentRoom_ThrowsNotFoundException()
    {
        // Arrange
        var query = new GetRoomByIdQuery(Guid.NewGuid());

        // Act
        Func<Task> act = () => _handler.Handle(query,CancellationToken.None).AsTask();

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}