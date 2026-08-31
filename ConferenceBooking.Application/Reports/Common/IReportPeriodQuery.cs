namespace ConferenceBooking.Application.Reports.Common;

public interface IReportPeriodQuery
{
    DateTime PeriodStart { get; }
    DateTime PeriodEnd { get; }
}