using ConferenceBooking.Domain.Common;

namespace ConferenceBooking.Domain.ValueObjects;


// Проміжок часу бронювання, перевірки коректності та перетину з іншими проміжками

public sealed class TimeRange : IEquatable<TimeRange>
{
    public DateTime Start { get; }
    public DateTime End { get; }

    private TimeRange(DateTime start, DateTime end)
    {
        Start = start;
        End = end;
    }

    public static TimeRange Create(DateTime start, DateTime end)
    {
        if (end <= start)
        {
            throw new ArgumentException(DomainErrorMessages.EndMustBeAfterStart);
        }
            

        if (start.Date != end.Date)
        {
            throw new ArgumentException(DomainErrorMessages.SameDayBookingOnly);
        }
          
        return new TimeRange(start, end);
    }

    public TimeSpan Duration
    {
        get { return End - Start; }
    }
    // Перевірка, чи зал уже зайнятий у цей час
    public bool Overlaps(TimeRange other)
    {
        return Start < other.End && other.Start < End;
    }
        
    public bool Equals(TimeRange? other)
    {
        return other is not null && Start == other.Start && End == other.End;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as TimeRange);
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(Start, End);
    }
    public override string ToString()
    {
        return $"{Start:HH:mm} - {End:HH:mm} ({Start:d})";
    }
}