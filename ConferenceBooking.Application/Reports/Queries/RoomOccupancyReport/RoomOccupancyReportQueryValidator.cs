using ConferenceBooking.Application.Common.ErrorMessages;
using FluentValidation;

namespace ConferenceBooking.Application.Reports.Queries.RoomOccupancyReport;

public class RoomOccupancyReportQueryValidator : AbstractValidator<RoomOccupancyReportQuery>
{
    public RoomOccupancyReportQueryValidator()
    {
        RuleFor(x => x.PeriodEnd)
            .GreaterThan(x => x.PeriodStart)
            .WithMessage(ReportErrorMessages.PeriodEndMustBeAfterStart);
    }
}