using ConferenceBooking.Domain.Common;
using ConferenceBooking.Domain.ValueObjects;

namespace ConferenceBooking.Domain.Entities;

// Послуга, яку можна замовити разом із залом
public class Service : AggregateRoot
{
    public string Name { get; private set; } = null!;
    public Money Price { get; private set; } = null!;
    public bool IsActive { get; private set; }

    private Service() { }

    public static Service Create(string name, Money price)
    {
        ValidateName(name);

        return new Service
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Price = price,
            IsActive = true
        };
    }

    public void UpdatePrice(Money newPrice)
    {
        Price = newPrice;
    }
        
    public void Rename(string newName)
    {
        ValidateName(newName);
        Name = newName.Trim();
    }

    public void Deactivate()
    {
        IsActive = false;
    }
       
    public void Activate() 
    {
        IsActive = true;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(DomainErrorMessages.ServiceNameRequired, nameof(name));
        }
           
    }
}