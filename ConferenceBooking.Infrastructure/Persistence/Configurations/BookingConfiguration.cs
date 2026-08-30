using ConferenceBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConferenceBooking.Infrastructure.Persistence.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("Bookings");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.RoomId).IsRequired();

        builder.OwnsOne(b => b.TimeRange, tr =>
        {
            tr.Property(t => t.Start)
                .HasColumnName("StartTime")
                .IsRequired();

            tr.Property(t => t.End)
                .HasColumnName("EndTime")
                .IsRequired();

           
            tr.HasIndex(t => new { t.Start, t.End });
        });

        builder.OwnsOne(b => b.TotalPrice, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("TotalPrice")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.Property(b => b.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(b => b.CreatedAtUtc).IsRequired();

        
        builder.Property<List<Guid>>("_selectedServiceIds")
            .HasColumnName("SelectedServiceIds")
            .HasConversion(
                ids => string.Join(',', ids),
                str => str == "" ? new List<Guid>() : 
                str.Split(',', StringSplitOptions.None).Select(Guid.Parse).ToList())
            .Metadata.SetValueComparer(new Microsoft.EntityFrameworkCore.ChangeTracking.
            ValueComparer<List<Guid>>(
                (a, b) => a!.SequenceEqual(b!),
                a => a.Aggregate(0, (hash, id) => HashCode.Combine(hash, id)),
                a => a.ToList()));

        // Захист від "загубленого оновлення" при одночасних змінах
        builder.Property<byte[]>("RowVersion")
            .IsRowVersion();

        builder.HasIndex(b => b.RoomId);
    }
}