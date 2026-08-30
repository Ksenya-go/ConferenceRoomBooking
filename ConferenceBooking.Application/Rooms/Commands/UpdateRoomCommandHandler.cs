using ConferenceBooking.Application.Common.ErrorMessages;
using ConferenceBooking.Application.Common.Exceptions;
using ConferenceBooking.Application.Common.Interfaces;
using ConferenceBooking.Domain.Entities;
using ConferenceBooking.Domain.ValueObjects;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace ConferenceBooking.Application.Rooms.Commands.UpdateRoom;

public class UpdateRoomCommandHandler : IRequestHandler<UpdateRoomCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public UpdateRoomCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<Unit> Handle(UpdateRoomCommand request, CancellationToken cancellationToken)
    {
        var room = await _context.Rooms
            .FirstOrDefaultAsync(r => r.Id == request.RoomId, cancellationToken);

        if (room is null)
        {
            throw new NotFoundException(RoomErrorMessages.RoomNotFound(request.RoomId));
        }
            
        room.UpdateDetails(request.Name, request.Capacity, Money.Uah(request.BaseHourlyRate));

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}