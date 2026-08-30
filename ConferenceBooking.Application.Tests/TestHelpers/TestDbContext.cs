using ConferenceBooking.Application.Common.Interfaces;
using ConferenceBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConferenceBooking.Application.Tests;

public class TestDbContext : DbContext, IApplicationDbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }

    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<Booking> Bookings => Set<Booking>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Room>(b =>
        {
            b.OwnsOne(r => r.BaseHourlyRate, money =>
            {
                money.Property(m => m.Amount);
                money.Property(m => m.Currency);
            });

            b.Metadata.FindNavigation(nameof(Room.Services))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);
            b.HasMany(r => r.Services).WithOne().HasForeignKey(rs => rs.RoomId);
        });

        modelBuilder.Entity<Service>(b =>
        {
            b.OwnsOne(s => s.Price, money =>
            {
                money.Property(m => m.Amount);
                money.Property(m => m.Currency);
            });
        });

        modelBuilder.Entity<Booking>(b =>
        {
            b.OwnsOne(bk => bk.TimeRange, tr =>
            {
                tr.Property(t => t.Start);
                tr.Property(t => t.End);
            });

            b.OwnsOne(bk => bk.TotalPrice, money =>
            {
                money.Property(m => m.Amount);
                money.Property(m => m.Currency);
            });

            b.Property<List<Guid>>("_selectedServiceIds")
                .HasConversion(
                    ids => string.Join(',', ids),
                    str => str == "" ? new List<Guid>() : str.Split(',', StringSplitOptions.None).
                    Select(Guid.Parse).ToList());
        });

        modelBuilder.Entity<RoomService>().HasKey(rs => new { rs.RoomId, rs.ServiceId });

        base.OnModelCreating(modelBuilder);
    }
}