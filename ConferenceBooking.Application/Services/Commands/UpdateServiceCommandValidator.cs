using ConferenceBooking.Application.Common.ErrorMessages;
using FluentValidation;

namespace ConferenceBooking.Application.Services.Commands;

public class UpdateServiceCommandValidator : AbstractValidator<UpdateServiceCommand>
{
    public UpdateServiceCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(ServiceErrorMessages.NameRequired)
            .MaximumLength(200).WithMessage(ServiceErrorMessages.NameTooLong);

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage(ServiceErrorMessages.PriceMustBePositive);
    }
}