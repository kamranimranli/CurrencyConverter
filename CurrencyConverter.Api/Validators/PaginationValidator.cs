using CurrencyConverter.Api.Models;
using FluentValidation;

namespace CurrencyConverter.Api.Validators;

public class PaginationValidator<T> : AbstractValidator<T> where T : Pagination
{
    public PaginationValidator()
    {
        RuleFor(x => x.Page)
        .GreaterThan(0).WithMessage("Page must be a positive number");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("PageSize must be a positive number");
    }
}
