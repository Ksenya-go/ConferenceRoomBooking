namespace ConferenceBooking.Application.Common.ErrorMessages;

public static class ServiceErrorMessages
{
    public const string NameRequired = "Назва послуги не може бути порожньою";
    public const string NameTooLong = "Назва послуги не може перевищувати 200 символів";
    public const string PriceMustBePositive = "Ціна послуги має бути більшою за нуль";
}