using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using TravelGuideApi.Application.Interfaces;
using TravelGuideApi.Domain.Models;

namespace TravelGuideApi.Application.Features.Budget.Queries.Calculate;

/// <summary>
/// Handles <see cref="BudgetCalculateQuery"/> queries.
/// </summary>
public class BudgetCalculateQueryHandler(
    IExchangeRateCache exchangeRateCache,
    ICountryCache countryCache,
    ILogger<BudgetCalculateQueryHandler> logger)
    : IRequestHandler<BudgetCalculateQuery, List<BudgetCalculationResult>>
{
    private readonly IExchangeRateCache _exchangeRateCache = exchangeRateCache;
    private readonly ICountryCache _countryCache = countryCache;
    private readonly ILogger<BudgetCalculateQueryHandler> _logger = logger;

    public async Task<List<BudgetCalculationResult>> Handle(
        BudgetCalculateQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Calculating trip budget for {TripDays} days with {HomeCurrency} across {DestinationCount} countries",
            request.TripDays,
            request.HomeCurrency,
            request.DestinationCountries.Count);

        Dictionary<string, decimal>? rates = await _exchangeRateCache.GetRatesAsync(request.HomeCurrency);

        if (rates == null)
        {
            throw new InvalidOperationException(
                $"Exchange rates not available for currency: {request.HomeCurrency}");
        }

        decimal totalBudgetHome = request.DailyBudget * request.TripDays;
        var results = new List<BudgetCalculationResult>();

        foreach (string countryCode in request.DestinationCountries)
        {
            var country = await _countryCache.GetByCodeAsync(countryCode);

            if (country == null)
            {
                throw new ArgumentException($"Country not found: {countryCode}");
            }

            if (!rates.TryGetValue(country.CurrencyCode, out decimal rate))
            {
                throw new InvalidOperationException(
                    $"Exchange rate not available for currency: {country.CurrencyCode}");
            }

            results.Add(new BudgetCalculationResult
            {
                CountryCode = country.Code,
                CountryName = country.Name,
                DestinationCurrency = country.CurrencyCode,
                ExchangeRate = Math.Round(rate, 4),
                DailyBudgetHome = request.DailyBudget,
                TripDays = request.TripDays,
                TotalBudgetHome = totalBudgetHome,
                TotalBudgetLocal = Math.Round(totalBudgetHome * rate, 2),
                DailyLocalAmount = Math.Round(request.DailyBudget * rate, 2),
                DataFreshness = "Live",
                RateTimestamp = DateTime.UtcNow
            });
        }

        return results;
    }
}
