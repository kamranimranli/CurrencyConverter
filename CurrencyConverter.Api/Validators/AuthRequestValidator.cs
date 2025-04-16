using CurrencyConverter.Api.Models;
using FluentValidation;

namespace CurrencyConverter.Api.Validators;

public class AuthRequestValidator : AbstractValidator<AuthRequest>
{
    public AuthRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.")
            .MinimumLength(3).WithMessage("UserId must be at least 3 characters.")
            .MaximumLength(50).WithMessage("UserId must not exceed 50 characters.");

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required.");
    }
}
