using ConferenceBooking.Domain.Entities;

namespace ConferenceBooking.Application.Services.Dtos;

public record ServiceDto(Guid Id, string Name, decimal Price, bool IsActive)
{
    public static ServiceDto FromEntity(Service service)
    {
        return new ServiceDto(service.Id, service.Name, service.Price.Amount, service.IsActive);
    }
       
}