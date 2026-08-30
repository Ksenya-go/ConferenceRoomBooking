namespace ConferenceBooking.Application.Reports.Dtos;

public record PopularServicesReportDto(
    Guid ServiceId,
    string ServiceName,
    int TimesOrdered,
    decimal TotalRevenue);