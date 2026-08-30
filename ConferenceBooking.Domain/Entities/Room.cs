using ConferenceBooking.Domain.Common;
using ConferenceBooking.Domain.ValueObjects;

namespace ConferenceBooking.Domain.Entities;

// Конференц-зал
public class Room : AggregateRoot
{
    private readonly List<RoomService> _services = new();

    public string Name { get; private set; } = null!;
    public int Capacity { get; private set; }
    public Money BaseHourlyRate { get; private set; } = null!;
    public bool IsActive { get; private set; }

    public IReadOnlyCollection<RoomService> Services => _services.AsReadOnly();

    private Room() { }

    public static Room Create(string name, int capacity, Money baseHourlyRate)
    {
        ValidateName(name);
        ValidateCapacity(capacity);

        return new Room
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Capacity = capacity,
            BaseHourlyRate = baseHourlyRate,
            IsActive = true
        };
    }

    public void UpdateDetails(string name, int capacity, Money baseHourlyRate)
    {
        ValidateName(name);
        ValidateCapacity(capacity);

        Name = name.Trim();
        Capacity = capacity;
        BaseHourlyRate = baseHourlyRate;
    }

    public void AddService(Service service)
    {
        if (_services.Any(rs => rs.ServiceId == service.Id))
        {
            throw new InvalidOperationException(DomainErrorMessages.
                ServiceAlreadyAddedToRoom(service.Name));
        }

        _services.Add(RoomService.Create(Id, service.Id));
    }

    public void RemoveService(Guid serviceId)
    {
        var link = _services.FirstOrDefault(rs => rs.ServiceId == serviceId);
        if (link is null)
        {
            throw new InvalidOperationException(DomainErrorMessages.ServiceNotLinkedToRoom);
        }
        _services.Remove(link);
    }

    public bool SupportsCapacity(int requiredCapacity) => Capacity >= requiredCapacity;

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
            throw new ArgumentException(DomainErrorMessages.RoomNameRequired, nameof(name));
        }
            
    }

    private static void ValidateCapacity(int capacity)
    {
        if (capacity <= 0)
        {
            throw new ArgumentException(DomainErrorMessages.RoomCapacityMustBePositive, nameof(capacity));
        }
            
    }
}