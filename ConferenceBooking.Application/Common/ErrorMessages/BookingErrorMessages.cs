namespace ConferenceBooking.Application.Common.ErrorMessages;

public static class BookingErrorMessages
{
    public const string RoomIdRequired = "ID залу є обов'язковим";
    public const string EndMustBeAfterStart = "Час завершення має бути пізніше за час початку";
    public const string CannotBookInThePast = "Неможливо забронювати зал у минулому";
    public const string ServiceIdRequired = "ID послуги не може бути порожнім";
    public static string RoomInactive(string roomName) => $"Зал '{roomName}' " +
        $"наразі недоступний для бронювання";
    public static string ServicesNotAvailable(string roomName, IEnumerable<Guid> serviceIds) =>
        $"Наступні послуги недоступні для залу '{roomName}': {string.Join(", ", serviceIds)}";
    public const string RoomAlreadyBooked = "Зал вже заброньований на цей проміжок часу";
}