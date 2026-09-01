using ConferenceBooking.Application.Common.ErrorMessages;
using FluentValidation;

namespace ConferenceBooking.Application.Reports.Common;

public abstract class ReportPeriodQueryValidator<T> : AbstractValidator<T>
    where T : IReportPeriodQuery
{
    protected ReportPeriodQueryValidator()
    {
        RuleFor(x => x.PeriodEnd)
            .GreaterThan(x => x.PeriodStart)
            .WithMessage(ReportErrorMessages.PeriodEndMustBeAfterStart);
    }
}