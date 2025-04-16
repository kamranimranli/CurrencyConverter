using CurrencyConverter.Api.Models;
using FluentValidation;

namespace CurrencyConverter.Api.Validators;

public class HistoricalRatesRequestValidator : PaginationValidator<HistoricalRatesRequest>
{
    public HistoricalRatesRequestValidator()
    {
        RuleFor(x => x.From)
            .NotNull().WithMessage("'from' date is required");

        RuleFor(x => x.To)
            .NotNull().WithMessage("'to' date is required");
    }
}
