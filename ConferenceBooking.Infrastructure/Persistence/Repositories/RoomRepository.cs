using ConferenceBooking.Domain.Entities;
using ConferenceBooking.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ConferenceBooking.Infrastructure.Persistence.Repositories;

public class RoomRepository : IRoomRepository
{
    private readonly AppDbContext _context;

    public RoomRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Room?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    { 
        return await _context.Rooms
            .Include(r => r.Services)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<List<Room>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Rooms
            .Include(r => r.Services)
            .Where(r => r.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Room>> GetAvailableAsync(int requiredCapacity, CancellationToken cancellationToken = default)
    {
        return await _context.Rooms
            .Include(r => r.Services)
            .Where(r => r.IsActive && r.Capacity >= requiredCapacity)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Room room, CancellationToken cancellationToken = default)
    {
        await _context.Rooms.AddAsync(room, cancellationToken);
    }   
    public void Update(Room room)
    {
        _context.Rooms.Update(room);
    }
    public void Delete(Room room)
    {
        _context.Rooms.Remove(room);
    }
}