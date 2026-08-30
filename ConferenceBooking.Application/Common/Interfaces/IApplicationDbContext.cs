using ConferenceBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConferenceBooking.Application.Common.Interfaces;

// Абстракція над EF Core DbContext, щоб Application-шар не залежав напряму від Infrastructure/EF Core.

public interface IApplicationDbContext
{
    DbSet<Room> Rooms { get; }
    DbSet<Service> Services { get; }
    DbSet<Booking> Bookings { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}