using ConferenceBooking.Domain.Entities;
using ConferenceBooking.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ConferenceBooking.Infrastructure.Persistence.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly AppDbContext _context;

    public BookingRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Bookings.FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task<List<Booking>> GetOverlappingAsync(
        Guid roomId, DateTime start, DateTime end, CancellationToken cancellationToken = default)
    {
        return await _context.Bookings
            .Where(b => b.RoomId == roomId)
            .Where(b => b.Status == Domain.Enums.BookingStatus.Confirmed)
            .Where(b => b.TimeRange.Start < end && start < b.TimeRange.End)
            .ToListAsync(cancellationToken);
    }   

    public async Task<List<Booking>> GetByRoomIdAsync(Guid roomId, 
        CancellationToken cancellationToken = default)
    {
        return await _context.Bookings.Where(b => b.RoomId == roomId).ToListAsync(cancellationToken);
    }

    public async Task<List<Booking>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Bookings.ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        await _context.Bookings.AddAsync(booking, cancellationToken);
    }
}