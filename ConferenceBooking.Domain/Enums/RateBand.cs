namespace ConferenceBooking.Domain.Enums;


// Визначає тариф для певного проміжку часу та його вплив на вартість оренди

public enum RateBand
{
    // 06:00–09:00, знижка 10%
    EarlyMorning,

    // 09:00–12:00 та 14:00–18:00, базова вартість
    Standard,

    // 12:00–14:00, націнка 15%
    Peak,

    // 18:00–23:00, знижка 20%
    Evening
}