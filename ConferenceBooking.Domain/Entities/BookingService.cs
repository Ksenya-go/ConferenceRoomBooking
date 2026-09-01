using ConferenceBooking.Domain.ValueObjects;

namespace ConferenceBooking.Domain.Entities;

// Знімок послуги на момент бронювання: зберігає назву та ціну такими,
// якими вони були під час бронювання, незалежно від того, що станеться з довідником послуг пізніше
public class BookingService
{
    private decimal _priceAmount;
    private string _priceCurrency = null!;

    public Guid BookingId { get; private set; }
    public Guid ServiceId { get; private set; }
    public string ServiceName { get; private set; } = null!;

    public Money PriceAtBooking => Money.Restore(_priceAmount, _priceCurrency);

    private BookingService() { }

    public static BookingService Create(
        Guid bookingId, Guid serviceId, string serviceName, Money priceAtBooking)
    {
        return new BookingService
        {
            BookingId = bookingId,
            ServiceId = serviceId,
            ServiceName = serviceName,
            _priceAmount = priceAtBooking.Amount,
            _priceCurrency = priceAtBooking.Currency
        };
    }
}