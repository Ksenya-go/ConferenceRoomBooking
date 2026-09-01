using ConferenceBooking.Application.Common.Exceptions;
using ConferenceBooking.Application.Rooms.Commands;
using ConferenceBooking.Domain.Entities;
using ConferenceBooking.Domain.ValueObjects;
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

    [Fact]
    public async Task Handle_WithServiceIds_LinksServicesToRoom()
    {
        // Arrange
        var wifi = Service.Create("Wi-Fi", Money.Uah(300));
        var projector = Service.Create("Проєктор", Money.Uah(500));
        await _context.Services.AddRangeAsync(wifi, projector);
        await _context.SaveChangesAsync();

        var command = new CreateRoomCommand("Зал D", 40, 1800, new List<Guid> { wifi.Id, projector.Id });

        // Act
        var roomId = await _handler.Handle(command, CancellationToken.None);

        // Assert
        var room = await _context.Rooms
            .Include(r => r.Services)
            .FirstAsync(r => r.Id == roomId);

        room.Services.Should().HaveCount(2);
        room.Services.Select(rs => rs.ServiceId).Should().BeEquivalentTo(new[] { wifi.Id, projector.Id });
    }

    [Fact]
    public async Task Handle_WithNonExistentServiceId_ThrowsNotFoundException()
    {
        // Arrange
        var nonExistentServiceId = Guid.NewGuid();
        var command = new CreateRoomCommand("Зал E", 20, 1000, new List<Guid> { nonExistentServiceId });
        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);
        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }


}