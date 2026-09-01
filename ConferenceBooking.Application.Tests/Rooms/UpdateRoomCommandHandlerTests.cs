using ConferenceBooking.Application.Common.Exceptions;
using ConferenceBooking.Application.Rooms.Commands;
using ConferenceBooking.Application.Rooms.Commands.UpdateRoom;
using ConferenceBooking.Domain.Entities;
using ConferenceBooking.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ConferenceBooking.Application.Tests.Rooms;

public class UpdateRoomCommandHandlerTests : IDisposable
{
    private readonly TestDbContext _context;
    private readonly UpdateRoomCommandHandler _handler;

    public UpdateRoomCommandHandlerTests()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new TestDbContext(options);
        _handler = new UpdateRoomCommandHandler(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task Handle_ExistingRoom_UpdatesFields()
    {
        // Arrange
        var room = Room.Create("Зал А",50,Money.Uah(2000));

        await _context.Rooms.AddAsync(room);
        await _context.SaveChangesAsync();

        var command = new UpdateRoomCommand(room.Id,"Зал А (новий)",60,2500);

        // Act
        await _handler.Handle(command,CancellationToken.None);

        // Assert
        var updatedRoom = await _context.Rooms.FirstAsync(r => r.Id == room.Id);

        updatedRoom.Name.Should().Be("Зал А (новий)");
        updatedRoom.Capacity.Should().Be(60);
        updatedRoom.BaseHourlyRate.Amount.Should().Be(2500);
    }

    [Fact]
    public async Task Handle_NonExistentRoom_ThrowsNotFoundException()
    {
        // Arrange
        var command = new UpdateRoomCommand(Guid.NewGuid(),"Зал",50,2000);

        // Act
        Func<Task> act = () => _handler.Handle(command,CancellationToken.None).AsTask();

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}