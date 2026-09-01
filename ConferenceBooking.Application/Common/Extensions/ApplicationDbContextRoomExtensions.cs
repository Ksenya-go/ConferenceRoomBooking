using ConferenceBooking.Application.Common.ErrorMessages;
using ConferenceBooking.Application.Common.Exceptions;
using ConferenceBooking.Application.Common.Interfaces;
using ConferenceBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConferenceBooking.Application.Common.Extensions;

public static class ApplicationDbContextRoomExtensions
{
    public static async Task<Room> GetRoomOrThrowAsync(
        this IApplicationDbContext context,
        Guid roomId,
        CancellationToken cancellationToken,
        bool includeServices = false)
    {
        var query = context.Rooms.AsQueryable();

        if (includeServices)
        {
            query = query.Include(r => r.Services);
        }

        var room = await query.FirstOrDefaultAsync(r => r.Id == roomId, cancellationToken);

        if (room is null)
        {
            throw new NotFoundException(RoomErrorMessages.RoomNotFound(roomId));
        }

        return room;
    }
}