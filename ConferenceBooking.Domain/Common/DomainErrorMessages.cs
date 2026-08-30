namespace ConferenceBooking.Domain.Common;

public static class DomainErrorMessages
{
    // Room
    public static string ServiceAlreadyAddedToRoom(string serviceName) =>
        $"Послуга '{serviceName}' вже додана до цього залу";
    public const string ServiceNotLinkedToRoom = "Ця послуга не прив'язана до залу";
    public const string RoomNameRequired = "Назва залу не може бути порожньою";
    public const string RoomCapacityMustBePositive = "Місткість залу має бути більшою за нуль";

    // Service
    public const string ServiceNameRequired = "Назва послуги не може бути порожньою";

    // TimeRange
    public const string EndMustBeAfterStart = "Час завершення має бути пізніше за час початку";
    public const string SameDayBookingOnly = "Бронювання не може тривати через опівніч " +
        "(поки не підтримується)";

    // Money
    public const string AmountCannotBeNegative = "Сума не може бути від'ємною";
    public const string CurrencyMismatch = "Неможливо оперувати сумами в різних валютах";
    public const string MultiplierCannotBeNegative = "Множник не може бути від'ємним";

    // Booking
    public const string BookingAlreadyCancelled = "Бронювання вже скасовано";
    public static string BookingOutsideScheduleRange(TimeOnly scheduleStart, TimeOnly scheduleEnd) =>
    $"Бронювання можливе лише в діапазоні {scheduleStart}–{scheduleEnd}";
}