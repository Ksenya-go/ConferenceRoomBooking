using ConferenceBooking.Domain.Common;
using ConferenceBooking.Domain.Enums;
using ConferenceBooking.Domain.ValueObjects;

namespace ConferenceBooking.Domain.Entities;

public class Booking : AggregateRoot
{
    private readonly List<BookingService> _services = new();

    public Guid RoomId { get; private set; }
    public TimeRange TimeRange { get; private set; } = null!;
    public Money TotalPrice { get; private set; } = null!;
    public BookingStatus Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    // Послуги, обрані під час бронювання, зі знімком назви/ціни на той момент
    public IReadOnlyCollection<BookingService> Services => _services.AsReadOnly();

    // Похідна властивість для місць, яким потрібні лише Id (наприклад, старі виклики/звіти)
    public IReadOnlyCollection<Guid> SelectedServiceIds => _services.Select(s => s.ServiceId).ToList();

    private Booking() { }

    public static Booking Create(
        Guid roomId,
        TimeRange timeRange,
        IEnumerable<Service> selectedServices,
        Money totalPrice)
    {
        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            RoomId = roomId,
            TimeRange = timeRange,
            TotalPrice = totalPrice,
            Status = BookingStatus.Confirmed,
            CreatedAtUtc = DateTime.UtcNow
        };

        // якщо клієнт випадково передав одну послугу двічі - DistinctBy
        foreach (var service in selectedServices.DistinctBy(s => s.Id))
        {
            booking._services.Add(
                BookingService.Create(booking.Id, service.Id, service.Name, service.Price));
        }

        return booking;
    }

    public void Cancel()
    {
        if (Status == BookingStatus.Cancelled)
        {
            throw new InvalidOperationException(DomainErrorMessages.BookingAlreadyCancelled);
        }

        Status = BookingStatus.Cancelled;
    }

    public bool OverlapsWith(TimeRange other)
    {
        return TimeRange.Overlaps(other);
    }
}