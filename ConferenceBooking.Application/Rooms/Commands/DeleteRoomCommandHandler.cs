using ConferenceBooking.Application.Common.ErrorMessages;
using ConferenceBooking.Application.Common.Exceptions;
using ConferenceBooking.Application.Common.Extensions;
using ConferenceBooking.Application.Common.Interfaces;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace ConferenceBooking.Application.Rooms.Commands.DeleteRoom;

public class DeleteRoomCommandHandler : IRequestHandler<DeleteRoomCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public DeleteRoomCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<Unit> Handle(DeleteRoomCommand request, CancellationToken cancellationToken)
    {
        var room = await _context.GetRoomOrThrowAsync(request.RoomId, cancellationToken);

        room.Deactivate();

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}