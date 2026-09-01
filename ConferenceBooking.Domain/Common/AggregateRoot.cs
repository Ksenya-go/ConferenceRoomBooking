namespace ConferenceBooking.Domain.Common;

// Базовий клас для сутностей, визначає спільний Id та порівняння сутностей за ідентичністю

public abstract class AggregateRoot
{
    public Guid Id { get; protected set; }

    // Сутності є однаковими, якщо мають однаковий тип та Id.
    public override bool Equals(object? obj)
    {
        if (obj is not AggregateRoot other)
        {
            return false;
        }

        // Якщо це той самий об'єкт у пам'яті — вони рівні.
        if (ReferenceEquals(this, other))
        {
            return true;
        }

        if (GetType() != other.GetType())
        {
            return false;
        }

        // Об'єкти без Id ще не мають визначеної ідентичності.
        if (Id == Guid.Empty || other.Id == Guid.Empty)
        {
            return false;
        }

        return Id == other.Id;
    }

    public static bool operator ==(AggregateRoot? left, AggregateRoot? right)
    {
        return left is null ? right is null : left.Equals(right);
    } 
        

    public static bool operator !=(AggregateRoot? left, AggregateRoot? right)
    {
        return !(left == right);
    }

    public override int GetHashCode() 
        {
            return HashCode.Combine(GetType(), Id);
        }
}