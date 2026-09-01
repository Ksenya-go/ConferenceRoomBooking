using ConferenceBooking.Application.Common.ErrorMessages;
using ConferenceBooking.Application.Common.Exceptions;
using ConferenceBooking.Application.Common.Interfaces;
using ConferenceBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConferenceBooking.Application.Common.Extensions;

public static class ApplicationDbContextServiceExtensions
{
    public static async Task<Service> GetServiceOrThrowAsync(
        this IApplicationDbContext context,
        Guid serviceId,
        CancellationToken cancellationToken)
    {
        var service = await context.Services
            .FirstOrDefaultAsync(s => s.Id == serviceId, cancellationToken);

        if (service is null)
        {
            throw new NotFoundException(RoomErrorMessages.ServiceNotFound(serviceId));
        }

        return service;
    }
}