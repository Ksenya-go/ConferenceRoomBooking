using ConferenceBooking.Application.Common.ErrorMessages;
using ConferenceBooking.Application.Common.Exceptions;
using ConferenceBooking.Application.Common.Extensions;
using ConferenceBooking.Application.Common.Interfaces;
using ConferenceBooking.Application.Rooms.Dtos;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace ConferenceBooking.Application.Rooms.Queries.GetRoomById;

public class GetRoomByIdQueryHandler : IRequestHandler<GetRoomByIdQuery, RoomDto>
{
    private readonly IApplicationDbContext _context;

    public GetRoomByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<RoomDto> Handle(GetRoomByIdQuery request, CancellationToken cancellationToken)
    {
        // Отримання ID залу
        var room = await _context.GetRoomOrThrowAsync(request.RoomId, cancellationToken, includeServices: true);
        // Отримання ID послуг, пов'язаних із залом
        var serviceIds = room.Services.Select(rs => rs.ServiceId).ToList();
        
        // Отримання всіх послуг, які відповідають залу
        var services = await _context.Services
            .Where(s => serviceIds.Contains(s.Id))
            .ToListAsync(cancellationToken);

        return RoomDto.FromEntity(room, services);
    }
}