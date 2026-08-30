using ConferenceBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConferenceBooking.Infrastructure.Persistence.Configurations;

public class RoomServiceConfiguration : IEntityTypeConfiguration<RoomService>
{
    public void Configure(EntityTypeBuilder<RoomService> builder)
    {
        builder.ToTable("RoomServices");

        // Складений ключ — один запис на пару (зал, послуга)
        builder.HasKey(rs => new { rs.RoomId, rs.ServiceId });

        builder.HasOne<Service>()
            .WithMany()
            .HasForeignKey(rs => rs.ServiceId)
            .OnDelete(DeleteBehavior.Restrict); // не дати видалити послугу, якщо вона десь використовується
    }
}