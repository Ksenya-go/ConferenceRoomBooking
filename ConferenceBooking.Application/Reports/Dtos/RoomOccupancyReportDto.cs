namespace ConferenceBooking.Application.Reports.Dtos;

public record RoomOccupancyReportDto(
    Guid RoomId,
    string RoomName,
    int TotalBookings,
    decimal TotalBookedHours,
    decimal OccupancyRatePercent);