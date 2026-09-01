using ConferenceBooking.Application.Services.Commands;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;


namespace ConferenceBooking.Application.Tests.Services;

public class CreateServiceCommandHandlerTests : IDisposable
{
    private readonly TestDbContext _context;
    private readonly CreateServiceCommandHandler _handler;

    public CreateServiceCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        _context = new TestDbContext(options);
        _handler = new CreateServiceCommandHandler(_context);
    }

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task Handle_ValidCommand_CreatesActiveService()
    {
        // Arrange
        var command = new CreateServiceCommand("Проєктор", 500);
        // Act
        var id = await _handler.Handle(command, CancellationToken.None);

        var saved = await _context.Services.FirstAsync(s => s.Id == id);
        // Assert
        saved.Name.Should().Be("Проєктор");
        saved.Price.Amount.Should().Be(500);
        saved.IsActive.Should().BeTrue();
    }
}