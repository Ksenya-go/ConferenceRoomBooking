using ConferenceBooking.Application.Common.Extensions;
using ConferenceBooking.Application.Common.Interfaces;
using Mediator;

namespace ConferenceBooking.Application.Rooms.Commands.AddServiceToRoom;

public class AddServiceToRoomCommandHandler : IRequestHandler<AddServiceToRoomCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public AddServiceToRoomCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<Unit> Handle(AddServiceToRoomCommand request, CancellationToken cancellationToken)
    {
        var room = await _context.GetRoomOrThrowAsync(request.RoomId, cancellationToken, includeServices: true);
        var service = await _context.GetServiceOrThrowAsync(request.ServiceId, cancellationToken);

        room.AddService(service);

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}