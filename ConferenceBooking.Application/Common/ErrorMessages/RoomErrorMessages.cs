namespace ConferenceBooking.Application.Common.ErrorMessages;

public static class RoomErrorMessages
{
    public const string NameRequired = "Назва залу не може бути порожньою";
    public const string NameTooLong = "Назва залу не може перевищувати 200 символів";
    public const string CapacityMustBePositive = "Місткість має бути більшою за нуль";
    public const string BaseHourlyRateMustBePositive = "Базова вартість має бути більшою за нуль";
    public const string RoomIdRequired = "ID залу є обов'язковим";
    public const string ServiceIdRequired = "ID послуги є обов'язковим";
    public const string ServiceAlreadyAdded = "Ця послуга вже додана до залу";
    public const string EndMustBeAfterStartForSearch = "Час завершення має бути пізніше за час початку";

    public static string RoomNotFound(Guid roomId) => $"Зал з ID '{roomId}' не знайдено";
    public static string ServiceNotFound(Guid serviceId) => $"Послугу з ID '{serviceId}' не знайдено";

}