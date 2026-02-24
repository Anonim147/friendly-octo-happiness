using FluentValidation;

namespace TravelGuideApi.Application.Features.CurrencyComparison.Queries.Compare;

/// <summary>
/// Defines validator for <see cref="CurrencyCompareQuery"/> queries.
/// </summary>
public class CurrencyCompareQueryValidator : AbstractValidator<CurrencyCompareQuery>
{
    public CurrencyCompareQueryValidator()
    {
        RuleFor(p => p.HomeCountryCode)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .NotNull()
            .Length(2).WithMessage("{PropertyName} must be a 2-character ISO country code.");

        RuleFor(p => p.DestinationCountryCode)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .NotNull()
            .Length(2).WithMessage("{PropertyName} must be a 2-character ISO country code.");
    }
}
