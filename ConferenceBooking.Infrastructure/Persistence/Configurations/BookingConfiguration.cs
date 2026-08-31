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

        builder.Ignore(b => b.SelectedServiceIds);

        builder.Navigation(b => b.Services)
                .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.OwnsMany(b => b.Services, bs =>
        {
            bs.ToTable("BookingServices");

            bs.WithOwner().HasForeignKey(x => x.BookingId);
            bs.HasKey(x => new { x.BookingId, x.ServiceId });

            bs.Property(x => x.ServiceName)
                .HasColumnName("ServiceName")
                .HasMaxLength(200)
                .IsRequired();

            bs.Ignore(x => x.PriceAtBooking);

           
            bs.Property<decimal>("_priceAmount")
                .HasColumnName("PriceAtBooking")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            bs.Property<string>("_priceCurrency")
                .HasColumnName("PriceCurrency")
                .HasMaxLength(3)
                .IsRequired();

            bs.HasOne<Service>()
                .WithMany()
                .HasForeignKey(x => x.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            bs.HasIndex(x => x.ServiceId);
        });

        // Захист від "загубленого оновлення" при одночасних змінах
        builder.Property<byte[]>("RowVersion")
            .IsRowVersion();

        builder.HasIndex(b => b.RoomId);
    }
}