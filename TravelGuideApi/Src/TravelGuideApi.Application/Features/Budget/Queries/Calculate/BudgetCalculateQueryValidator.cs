using FluentValidation;

namespace TravelGuideApi.Application.Features.Budget.Queries.Calculate;

/// <summary>
/// Defines validator for <see cref="BudgetCalculateQuery"/> queries.
/// </summary>
public class BudgetCalculateQueryValidator : AbstractValidator<BudgetCalculateQuery>
{
    public BudgetCalculateQueryValidator()
    {
        RuleFor(p => p.HomeCurrency)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .Length(3, 3).WithMessage("{PropertyName} must be a 3-character ISO currency code.");

        RuleFor(p => p.DailyBudget)
            .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.");

        RuleFor(p => p.TripDays)
            .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.")
            .LessThanOrEqualTo(365).WithMessage("{PropertyName} must not exceed 365 days.");

        RuleFor(p => p.DestinationCountries)
            .NotEmpty().WithMessage("{PropertyName} must contain at least one country.");

        RuleForEach(p => p.DestinationCountries)
            .Length(2, 2).WithMessage("Each destination country must be a 2-character ISO country code.");
    }
}
