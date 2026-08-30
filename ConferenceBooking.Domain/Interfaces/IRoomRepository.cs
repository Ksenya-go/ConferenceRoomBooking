using ConferenceBooking.Domain.Entities;

namespace ConferenceBooking.Domain.Interfaces;

public interface IRoomRepository
{
    Task<Room?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Room>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    Task<List<Room>> GetAvailableAsync(int requiredCapacity, CancellationToken cancellationToken = default);
    Task AddAsync(Room room, CancellationToken cancellationToken = default);
    void Update(Room room);
    void Delete(Room room);
}