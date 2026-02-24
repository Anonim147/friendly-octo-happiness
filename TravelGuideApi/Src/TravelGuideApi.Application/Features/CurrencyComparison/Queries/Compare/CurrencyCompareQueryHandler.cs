using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using TravelGuideApi.Application.Interfaces;
using TravelGuideApi.Domain.Entities;
using TravelGuideApi.Domain.Interfaces;
using TravelGuideApi.Domain.Models;

namespace TravelGuideApi.Application.Features.CurrencyComparison.Queries.Compare;

/// <summary>
/// Handles <see cref="CurrencyCompareQuery"/> queries.
/// </summary>
public class CurrencyCompareQueryHandler(
    ICountryApiService countryApiService,
    IExchangeRateApiService exchangeRateApiService,
    ICountryCache countryCache,
    ILogger<CurrencyCompareQueryHandler> logger)
    : IRequestHandler<CurrencyCompareQuery, CurrencyComparisonResult>
{
    private readonly ICountryApiService _countryApiService = countryApiService;
    private readonly IExchangeRateApiService _exchangeRateApiService = exchangeRateApiService;
    private readonly ICountryCache _countryCache = countryCache;
    private readonly ILogger<CurrencyCompareQueryHandler> _logger = logger;

    public async Task<CurrencyComparisonResult> Handle(
        CurrencyCompareQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Comparing currencies between {HomeCountryCode} and {DestinationCountryCode}",
            request.HomeCountryCode,
            request.DestinationCountryCode);

        // Get countries (try cache first, then API)
        CountryEntity? homeCountry = await _countryCache.GetByCodeAsync(request.HomeCountryCode)
            ?? await _countryApiService.GetCountryByCodeAsync(request.HomeCountryCode);

        CountryEntity? destinationCountry = await _countryCache.GetByCodeAsync(request.DestinationCountryCode)
            ?? await _countryApiService.GetCountryByCodeAsync(request.DestinationCountryCode);

        if (homeCountry == null)
        {
            throw new ArgumentException($"Country not found: {request.HomeCountryCode}", nameof(request.HomeCountryCode));
        }

        if (destinationCountry == null)
        {
            throw new ArgumentException($"Country not found: {request.DestinationCountryCode}", nameof(request.DestinationCountryCode));
        }

        // Get exchange rate
        decimal exchangeRate = await _exchangeRateApiService.GetExchangeRateAsync(
            homeCountry.CurrencyCode,
            destinationCountry.CurrencyCode);

        string strengthHint = DetermineStrengthHint(
            exchangeRate,
            homeCountry.CurrencyCode,
            destinationCountry.CurrencyCode);

        return new CurrencyComparisonResult
        {
            HomeCountry = homeCountry.Name,
            HomeCurrency = homeCountry.CurrencyCode,
            DestinationCountry = destinationCountry.Name,
            DestinationCurrency = destinationCountry.CurrencyCode,
            ExchangeRate = Math.Round(exchangeRate, 4),
            StrengthHint = strengthHint
        };
    }

    private static string DetermineStrengthHint(
        decimal exchangeRate,
        string homeCurrency,
        string destinationCurrency)
    {
        if (homeCurrency.Equals(destinationCurrency, StringComparison.OrdinalIgnoreCase))
        {
            return "Same currency";
        }

        return exchangeRate switch
        {
            > 1 => "Your money is stronger",
            < 1 => "Your money is weaker",
            _ => "Currencies are equal"
        };
    }
}
