using ConferenceBooking.Domain.Common;
using ConferenceBooking.Domain.Enums;
using ConferenceBooking.Domain.ValueObjects;

namespace ConferenceBooking.Domain.Entities;


public class Booking : AggregateRoot
{
    private readonly List<Guid> _selectedServiceIds = new();

    public Guid RoomId { get; private set; }
    public TimeRange TimeRange { get; private set; } = null!;
    public Money TotalPrice { get; private set; } = null!;
    public BookingStatus Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public IReadOnlyCollection<Guid> SelectedServiceIds => _selectedServiceIds.AsReadOnly();

    private Booking() { } 

    public static Booking Create(
        Guid roomId,
        TimeRange timeRange,
        IEnumerable<Guid> selectedServiceIds,
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

        // якщо клієнт випадково передав одну послугу двічі - Distinct
        booking._selectedServiceIds.AddRange(selectedServiceIds.Distinct());
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

    // Перевірка на перетин бронювання з іншим проміжком часу (чи вільний зал)
    public bool OverlapsWith(TimeRange other)
    {
        return TimeRange.Overlaps(other);
    }
}