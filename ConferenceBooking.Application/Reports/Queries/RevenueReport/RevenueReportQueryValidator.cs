using ConferenceBooking.Application.Common.ErrorMessages;
using FluentValidation;

namespace ConferenceBooking.Application.Reports.Queries.RevenueReport;

public class RevenueReportQueryValidator : AbstractValidator<RevenueReportQuery>
{
    public RevenueReportQueryValidator()
    {
        RuleFor(x => x.PeriodEnd)
            .GreaterThan(x => x.PeriodStart)
            .WithMessage(ReportErrorMessages.PeriodEndMustBeAfterStart);
    }
}