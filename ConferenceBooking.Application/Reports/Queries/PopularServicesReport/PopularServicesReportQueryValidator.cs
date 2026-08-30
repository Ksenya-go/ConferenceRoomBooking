using ConferenceBooking.Application.Common.ErrorMessages;
using FluentValidation;

namespace ConferenceBooking.Application.Reports.Queries.PopularServicesReport;

public class PopularServicesReportQueryValidator : AbstractValidator<PopularServicesReportQuery>
{
    public PopularServicesReportQueryValidator()
    {
        RuleFor(x => x.PeriodEnd)
            .GreaterThan(x => x.PeriodStart)
            .WithMessage(ReportErrorMessages.PeriodEndMustBeAfterStart);
    }
}