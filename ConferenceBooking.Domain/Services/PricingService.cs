using ConferenceBooking.Domain.Common;
using ConferenceBooking.Domain.Entities;
using ConferenceBooking.Domain.Enums;
using ConferenceBooking.Domain.ValueObjects;

namespace ConferenceBooking.Domain.Services;

// Розрахунок вартості оренди залу та додаткових послуг
public class PricingService
{
    // Визначає тариф для кожного проміжку часу протягом дня
    // PercentModifier: від'ємне значення — знижка, додатне — націнка, 0 — звичайна ціна
    private static readonly (TimeOnly Start, TimeOnly End, RateBand Band, decimal PercentModifier)
    [] RateSchedule =
    {
        (new TimeOnly(6, 0),  new TimeOnly(9, 0),  RateBand.EarlyMorning, -10m),
        (new TimeOnly(9, 0),  new TimeOnly(12, 0), RateBand.Standard,       0m),
        (new TimeOnly(12, 0), new TimeOnly(14, 0), RateBand.Peak,         15m),
        (new TimeOnly(14, 0), new TimeOnly(18, 0), RateBand.Standard,       0m),
        (new TimeOnly(18, 0), new TimeOnly(23, 0), RateBand.Evening,     -20m),
    };

    // Загальна вартість = оренда залу + всі обрані послуги
    public Money CalculateTotalPrice(Room room, TimeRange timeRange, IEnumerable<Service> selectedServices)
    {
        var roomCost = CalculateRoomCost(room.BaseHourlyRate, timeRange);
        var servicesCost = selectedServices
            .Select(s => s.Price)
            .Aggregate(Money.Zero(), (acc, price) => acc.Add(price));

        return roomCost.Add(servicesCost);
    }

    // Якщо бронювання захоплює кілька тарифних відрізків одразу, розрахунок кожного шматка відбувається
    // окремо за своєю ціною, а потім все складається в загальну суму
    private Money CalculateRoomCost(Money baseHourlyRate, TimeRange timeRange)
    {
        var total = Money.Zero();
        var startTime = TimeOnly.FromDateTime(timeRange.Start);
        var endTime = TimeOnly.FromDateTime(timeRange.End);

        ValidateFullyCovered(startTime, endTime);

        foreach (var (bandStart, bandEnd, _, modifier) in RateSchedule)
        {
            var overlapStart = MaxTime(startTime, bandStart);
            var overlapEnd = MinTime(endTime, bandEnd);

            if (overlapEnd <= overlapStart)
            {
                continue;
            }
           
            var overlapHours = (decimal)(overlapEnd - overlapStart).TotalHours;
            var segmentCost = baseHourlyRate
                .Multiply(overlapHours)
                .ApplyPercentage(modifier);

            total = total.Add(segmentCost);
        }

        return total;
    }

    // Перевірка бронювання на дозволений діапазон 06:00–23:00
    private void ValidateFullyCovered(TimeOnly start, TimeOnly end)
    {
        var scheduleStart = RateSchedule.First().Start;
        var scheduleEnd = RateSchedule.Last().End;

        if (start < scheduleStart || end > scheduleEnd)
        {
            throw new ArgumentException(
                DomainErrorMessages.BookingOutsideScheduleRange(scheduleStart, scheduleEnd));
        }
            
    }
    // Повертає пізніший із двох моментів часу
    private static TimeOnly MaxTime(TimeOnly a, TimeOnly b)
    {
        return a > b ? a : b;
    }
    // Повертає раніший із двох моментів часу
    private static TimeOnly MinTime(TimeOnly a, TimeOnly b)
    {
        return a < b ? a : b;
    }
}