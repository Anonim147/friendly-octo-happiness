using MediatR;
using TravelGuideApi.Domain.Models;

namespace TravelGuideApi.Application.Features.Budget.Queries.Calculate;

/// <summary>
/// Query to calculate a trip budget across one or more destination countries.
/// </summary>
/// <param name="HomeCurrency">ISO 4217 code of the home currency (e.g., "USD").</param>
/// <param name="DailyBudget">Daily budget amount in the home currency.</param>
/// <param name="TripDays">Number of days for the trip.</param>
/// <param name="DestinationCountries">List of ISO 3166-1 alpha-2 destination country codes.</param>
public record BudgetCalculateQuery(
    string HomeCurrency,
    decimal DailyBudget,
    int TripDays,
    List<string> DestinationCountries)
    : IRequest<List<BudgetCalculationResult>>;
