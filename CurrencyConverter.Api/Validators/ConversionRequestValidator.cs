using CurrencyConverter.Api.Models;
using FluentValidation;

namespace CurrencyConverter.Api.Validators;

public class ConversionRequestValidator : AbstractValidator<ConversionRequest>
{
    private static readonly string[] ForbiddenCurrencies = { "TRY", "PLN", "THB", "MXN" };

    public ConversionRequestValidator()
    {
        RuleFor(x => x.From)
            .NotEmpty().WithMessage("Source currency is required.")
            .Length(3).WithMessage("Currency code must be 3 letters.")
            .Matches("^[A-Z]{3}$").WithMessage("Currency code must be uppercase (e.g. USD)")
            .Must(code => !ForbiddenCurrencies.Contains(code))
            .WithMessage("Currency '{PropertyValue}' is not allowed for conversion.");

        RuleFor(x => x.To)
            .NotEmpty().WithMessage("Target currency is required.")
            .Length(3).WithMessage("Currency code must be 3 letters.")
            .Matches("^[A-Z]{3}$").WithMessage("Currency code must be uppercase (e.g. EUR)")
            .Must(code => !ForbiddenCurrencies.Contains(code))
            .WithMessage("Currency '{PropertyValue}' is not allowed for conversion.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than zero.");
    }
}
