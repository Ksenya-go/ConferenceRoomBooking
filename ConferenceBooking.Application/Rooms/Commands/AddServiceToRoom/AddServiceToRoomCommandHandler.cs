using ConferenceBooking.Application.Common.ErrorMessages;
using ConferenceBooking.Application.Common.Exceptions;
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
        // Отримання зали разом з його послугами
        var room = await _context.Rooms
            .Include(r => r.Services)
            .FirstOrDefaultAsync(r => r.Id == request.RoomId, cancellationToken);

        if (room is null)
        {
            throw new NotFoundException(RoomErrorMessages.RoomNotFound(request.RoomId));
        }
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