using ConferenceBooking.Application.Common.ErrorMessages;
using ConferenceBooking.Application.Common.Exceptions;
using ConferenceBooking.Application.Common.Extensions;
using ConferenceBooking.Application.Common.Interfaces;
using Mediator;
using Microsoft.EntityFrameworkCore;

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
        
        // Перевірка, чи існує послуга, яку потрібно додати до залу
        var service = await _context.Services
            .FirstOrDefaultAsync(s => s.Id == request.ServiceId, cancellationToken);

        if (service is null)
        {
            throw new NotFoundException(RoomErrorMessages.ServiceNotFound(request.ServiceId));
        }
         
        room.AddService(service);

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}