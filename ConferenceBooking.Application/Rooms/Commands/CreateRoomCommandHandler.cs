using ConferenceBooking.Application.Common.ErrorMessages;
using ConferenceBooking.Application.Common.Exceptions;
using ConferenceBooking.Application.Common.Interfaces;
using ConferenceBooking.Domain.Entities;
using ConferenceBooking.Domain.ValueObjects;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace ConferenceBooking.Application.Rooms.Commands;

public class CreateRoomCommandHandler : IRequestHandler<CreateRoomCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateRoomCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<Guid> Handle(CreateRoomCommand request, CancellationToken cancellationToken)
    {
        var room = Room.Create(
            request.Name,
            request.Capacity,
            Money.Uah(request.BaseHourlyRate));

        if (request.ServiceIds is { Count: > 0 })
        {
            await LinkServicesAsync(room, request.ServiceIds, cancellationToken);
        }

        await _context.Rooms.AddAsync(room, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return room.Id;
    }

    private async Task LinkServicesAsync(Room room, List<Guid> serviceIds, CancellationToken cancellationToken)
    {
        var services = await _context.Services
            .Where(s => serviceIds.Contains(s.Id))
            .ToListAsync(cancellationToken);

        var missingServiceId = serviceIds.FirstOrDefault(id => services.All(s => s.Id != id));
        if (missingServiceId != Guid.Empty)
        {
            throw new NotFoundException(RoomErrorMessages.ServiceNotFound(missingServiceId));
        }

        foreach (var service in services)
        {
            room.AddService(service);
        }
    }
}