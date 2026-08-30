using ConferenceBooking.Domain.Entities;

namespace ConferenceBooking.Domain.Interfaces;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    // Перевірити, чи є вже бронювання цього залу, що накладаються на вказаний час
    Task<List<Booking>> GetOverlappingAsync(
        Guid roomId, DateTime start, DateTime end, CancellationToken cancellationToken = default);

    Task<List<Booking>> GetByRoomIdAsync(Guid roomId, CancellationToken cancellationToken = default);
    Task<List<Booking>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Booking booking, CancellationToken cancellationToken = default);
}