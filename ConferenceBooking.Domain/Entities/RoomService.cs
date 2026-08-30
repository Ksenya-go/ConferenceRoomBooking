namespace ConferenceBooking.Domain.Entities;

// Зв'язок між залом і послугою, який показує доступні послуги в залі.
public class RoomService
{
    public Guid RoomId { get; private set; }
    public Guid ServiceId { get; private set; }

    private RoomService() { }

    public static RoomService Create(Guid roomId, Guid serviceId)
    {
        return new RoomService
        {
            RoomId = roomId,
            ServiceId = serviceId
        };
    }
}