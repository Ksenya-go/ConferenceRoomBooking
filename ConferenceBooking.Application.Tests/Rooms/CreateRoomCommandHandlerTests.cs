using ConferenceBooking.Application.Rooms.Commands;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ConferenceBooking.Application.Tests.Rooms;

public class CreateRoomCommandHandlerTests : IDisposable
{
    private readonly TestDbContext _context;
    private readonly CreateRoomCommandHandler _handler;

    public CreateRoomCommandHandlerTests()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new TestDbContext(options);
        _handler = new CreateRoomCommandHandler(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesRoomAndReturnsId()
    {
        // Arrange
        var command = new CreateRoomCommand("Зал А",50,2000);

        // Act
        var roomId = await _handler.Handle(command,CancellationToken.None);

        // Assert
        roomId.Should().NotBeEmpty();

        var savedRoom = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == roomId);

        savedRoom.Should().NotBeNull();
        savedRoom!.Name.Should().Be("Зал А");
        savedRoom.Capacity.Should().Be(50);
        savedRoom.IsActive.Should().BeTrue();
    }
}