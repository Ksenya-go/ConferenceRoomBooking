using ConferenceBooking.Domain.Common;

namespace ConferenceBooking.Domain.ValueObjects;


public sealed class Money : IEquatable<Money>
{
    public decimal Amount { get; }
    public string Currency { get; }

    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    // Відновлення Money після збереження в БД без повторної валідації
    public static Money Restore(decimal amount, string currency)
    {
        return new Money(amount, currency);
    }

    public static Money Uah(decimal amount)
    {
        if (amount < 0)
        {
            throw new ArgumentException(DomainErrorMessages.AmountCannotBeNegative, nameof(amount));
        }
      
        return new Money(amount, "UAH");
    }

    public static Money Zero()
    {
          return new(0, "UAH");
    }
    // Скласти дві суми (ціну залу + ціну послуг)
    public Money Add(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(Amount + other.Amount, Currency);
    }

    // Застосувати знижку чи націнку у відсотках
    public Money ApplyPercentage(decimal percentage)
    {
        var multiplier = 1 + percentage / 100m;
        return new Money(Math.Round(Amount * multiplier, 2), Currency);
    }
    // Помножити на кількість (ціну за годину на кількість годин)
    public Money Multiply(decimal factor)
    {
        if (factor < 0)
        {
            throw new ArgumentException(DomainErrorMessages.MultiplierCannotBeNegative, nameof(factor));
        }

        return new Money(Math.Round(Amount * factor, 2), Currency);
    }

    private void EnsureSameCurrency(Money other)
    {
        if (Currency != other.Currency)
        {
            throw new InvalidOperationException(DomainErrorMessages.CurrencyMismatch);
        }
            
    }

    public bool Equals(Money? other)
    {
        return other is not null && Amount == other.Amount && Currency == other.Currency;
    }

    public override bool Equals(object? obj) 
    {
        return Equals(obj as Money);
    }
    public override int GetHashCode() 
    {
        return HashCode.Combine(Amount, Currency);
    }
    public override string ToString()
    {
        return $"{Amount:F2} {Currency}";
    }
  
}