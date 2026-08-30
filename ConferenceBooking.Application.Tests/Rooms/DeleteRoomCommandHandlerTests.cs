using ConferenceBooking.Application.Common.Exceptions;
using ConferenceBooking.Application.Rooms.Commands.DeleteRoom;
using ConferenceBooking.Domain.Entities;
using ConferenceBooking.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ConferenceBooking.Application.Tests.Rooms;

public class DeleteRoomCommandHandlerTests : IDisposable
{
    private readonly TestDbContext _context;
    private readonly DeleteRoomCommandHandler _handler;

    public DeleteRoomCommandHandlerTests()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new TestDbContext(options);
        _handler = new DeleteRoomCommandHandler(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task Handle_ExistingRoom_DeactivatesRoom()
    {
        // Arrange
        var room = Room.Create("Зал А", 50, Money.Uah(2000));

        await _context.Rooms.AddAsync(room);
        await _context.SaveChangesAsync();

        var command = new DeleteRoomCommand(room.Id);

        // Act
        await _handler.Handle(command,CancellationToken.None);

        // Assert
        // Кімната не видаляється з БД фізично,переводиться в неактивний стан
        var deletedRoom = await _context.Rooms.FirstAsync(r => r.Id == room.Id);

        deletedRoom.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_NonExistentRoom_ThrowsNotFoundException()
    {
        // Arrange
        var command = new DeleteRoomCommand(Guid.NewGuid());

        // Act
        Func<Task> act = () => _handler.Handle(command,CancellationToken.None).AsTask();

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}