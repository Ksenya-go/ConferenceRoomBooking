using ConferenceBooking.Application.Bookings.Commands;
using ConferenceBooking.Application.Common.Exceptions;
using ConferenceBooking.Domain.Entities;
using ConferenceBooking.Domain.Services;
using ConferenceBooking.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;


namespace ConferenceBooking.Application.Tests.Bookings;

public class CreateBookingCommandHandlerTests : IDisposable
{
    private readonly TestDbContext _context;
    private readonly PricingService _pricingService;
    private readonly FakeBookingTransactionGuard _guard;
    private readonly CreateBookingCommandHandler _handler;

    public CreateBookingCommandHandlerTests()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new TestDbContext(options);
        _pricingService = new PricingService();
        _guard = new FakeBookingTransactionGuard();

        _handler = new CreateBookingCommandHandler(
            _context,
            _pricingService,
            _guard);
    }

    public void Dispose() 
    { 
        _context.Dispose(); 
    }

    private async Task<Room> SeedRoomWithServiceAsync(decimal hourlyRate = 1500)
    {
        var room = Room.Create("Зал C", 30, Money.Uah(hourlyRate));
        var wifi = Service.Create("Wi-Fi", Money.Uah(300));
        room.AddService(wifi);

        await _context.Services.AddAsync(wifi);
        await _context.Rooms.AddAsync(room);
        await _context.SaveChangesAsync(CancellationToken.None);

        return room;
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesBookingWithCorrectPrice()
    {
        // Arrange
        var room = await SeedRoomWithServiceAsync();

        var start = new DateTime(2026, 9, 1, 10, 0, 0);

        var command = new CreateBookingCommand(
            room.Id,
            start,
            start.AddHours(2),
            new List<Guid>());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.TotalPrice.Should().Be(3000);
        result.BookingId.Should().NotBeEmpty();
        var savedBooking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == result.BookingId);
        savedBooking.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_NonExistentRoom_ThrowsNotFoundException()
    {
        // Arrange
        var command = new CreateBookingCommand(
            Guid.NewGuid(),
            new DateTime(2026, 9, 1, 10, 0, 0),
            new DateTime(2026, 9, 1, 12, 0, 0),
            new List<Guid>());

        // Act
        Func<Task> act = () => _handler.Handle(command,CancellationToken.None).AsTask();

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_InactiveRoom_ThrowsBusinessRuleException()
    {
        // Arrange
        var room = await SeedRoomWithServiceAsync();

        room.Deactivate();

        await _context.SaveChangesAsync(CancellationToken.None);

        var command = new CreateBookingCommand(
            room.Id,
            new DateTime(2026, 9, 1, 10, 0, 0),
            new DateTime(2026, 9, 1, 12, 0, 0),
            new List<Guid>());

        // Act
        Func<Task> act = () => _handler.Handle(command, CancellationToken.None).AsTask();

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>();
    }

    [Fact]
    public async Task Handle_ServiceNotLinkedToRoom_ThrowsBusinessRuleException()
    {
        // Arrange
        var room = Room.Create("Зал без послуг",20,Money.Uah(1000));

        await _context.Rooms.AddAsync(room);
        await _context.SaveChangesAsync(CancellationToken.None);

        var command = new CreateBookingCommand(
            room.Id,
            new DateTime(2026, 9, 1, 10, 0, 0),
            new DateTime(2026, 9, 1, 12, 0, 0),
            new List<Guid> { Guid.NewGuid() });

        // Act
        Func<Task> act = () => _handler.Handle(command, CancellationToken.None).AsTask();

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>();
    }

    [Fact]
    public async Task Handle_OverlappingBooking_ThrowsBusinessRuleException()
    {
        // Arrange
        var room = await SeedRoomWithServiceAsync();

        var firstCommand = new CreateBookingCommand(
            room.Id,
            new DateTime(2026, 9, 1, 10, 0, 0),
            new DateTime(2026, 9, 1, 12, 0, 0),
            new List<Guid>());

        await _handler.Handle(firstCommand,CancellationToken.None);

        var secondCommand = new CreateBookingCommand(
            room.Id,
            new DateTime(2026, 9, 1, 11, 0, 0),
            new DateTime(2026, 9, 1, 13, 0, 0),
            new List<Guid>());

        // Act
        Func<Task> act = () => _handler.Handle(secondCommand,CancellationToken.None).AsTask();

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>();
    }

    [Fact]
    public async Task Handle_NonOverlappingBooking_Succeeds()
    {
        // Arrange
        var room = await SeedRoomWithServiceAsync();

        var firstCommand = new CreateBookingCommand(
            room.Id,
            new DateTime(2026, 9, 1, 10, 0, 0),
            new DateTime(2026, 9, 1, 12, 0, 0),
            new List<Guid>());

        await _handler.Handle(firstCommand,CancellationToken.None);

        var secondCommand = new CreateBookingCommand(
            room.Id,
            new DateTime(2026, 9, 1, 14, 0, 0),
            new DateTime(2026, 9, 1, 16, 0, 0),
            new List<Guid>());

        // Act
        var result = await _handler.Handle(secondCommand,CancellationToken.None);

        // Assert
        result.BookingId.Should().NotBeEmpty();

        var allBookings = await _context.Bookings.Where(b => b.RoomId == room.Id).ToListAsync();

        allBookings.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_WithSelectedService_IncludesServicePriceInTotal()
    {
        // Arrange
        var room = await SeedRoomWithServiceAsync(hourlyRate: 1500);

        var wifi = await _context.Services.FirstAsync();

        var command = new CreateBookingCommand(
            room.Id,
            new DateTime(2026, 9, 1, 10, 0, 0),
            new DateTime(2026, 9, 1, 12, 0, 0),
            new List<Guid> { wifi.Id });

        // Act
        var result = await _handler.Handle(command,CancellationToken.None);

        // Assert
        result.TotalPrice.Should().Be(3300);
    }
}