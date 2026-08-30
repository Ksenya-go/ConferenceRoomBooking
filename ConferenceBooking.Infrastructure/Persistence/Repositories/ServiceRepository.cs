using ConferenceBooking.Domain.Entities;
using ConferenceBooking.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ConferenceBooking.Infrastructure.Persistence.Repositories;

public class ServiceRepository : IServiceRepository
{
    private readonly AppDbContext _context;

    public ServiceRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Service?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Services.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }
   
    public async Task<List<Service>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        return await _context.Services.Where(s => ids.Contains(s.Id)).ToListAsync(cancellationToken);
    }

    public async Task<List<Service>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Services.Where(s => s.IsActive).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Service service, CancellationToken cancellationToken = default)
    {
        await _context.Services.AddAsync(service, cancellationToken);
    }   
}