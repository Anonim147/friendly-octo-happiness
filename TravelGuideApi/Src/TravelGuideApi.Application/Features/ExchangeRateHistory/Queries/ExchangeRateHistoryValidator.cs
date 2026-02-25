using System;
using FluentValidation;

namespace TravelGuideApi.Application.Features.ExchangeRateHistory.Queries;

/// <summary>
/// Defines validator for <see cref="ExchangeRateHistoryQuery"/> queries.
/// </summary>
public class ExchangeRateHistoryValidator : AbstractValidator<ExchangeRateHistoryQuery>
{
    private static readonly int[] AllowedDays = [30, 90, 365];

    public ExchangeRateHistoryValidator()
    {
        RuleFor(p => p.FromCurrency)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .NotNull()
            .Length(3).WithMessage("{PropertyName} must be a 3-character ISO currency code.");

        RuleFor(p => p.ToCurrency)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .NotNull()
            .Length(3).WithMessage("{PropertyName} must be a 3-character ISO currency code.");

        RuleFor(p => p.Days)
            .Must(d => Array.IndexOf(AllowedDays, d) >= 0)
            .WithMessage("{PropertyName} must be 30, 90, or 365.");
    }
}
