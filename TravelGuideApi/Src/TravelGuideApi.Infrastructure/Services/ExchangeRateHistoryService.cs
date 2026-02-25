using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TravelGuideApi.Domain.Interfaces;
using TravelGuideApi.Domain.Models;
using TravelGuideApi.Domain.Models.ExternalApi;

namespace TravelGuideApi.Infrastructure.Services;

/// <summary>
/// Service for fetching historical exchange rate data from the Frankfurter API.
/// </summary>
public class ExchangeRateHistoryService : IExchangeRateHistoryService
{
    private const string FrankfurterBaseUrl = "https://api.frankfurter.app";

    private readonly HttpClient _httpClient;
    private readonly ILogger<ExchangeRateHistoryService> _logger;

    public ExchangeRateHistoryService(
        HttpClient httpClient,
        ILogger<ExchangeRateHistoryService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<IReadOnlyList<ExchangeRateDataPoint>> GetHistoricalRatesAsync(
        string fromCurrency,
        string toCurrency,
        DateOnly startDate,
        DateOnly endDate)
    {
        var start = startDate.ToString("yyyy-MM-dd");
        var end = endDate.ToString("yyyy-MM-dd");
        var url = $"{FrankfurterBaseUrl}/{start}..{end}?from={fromCurrency.ToUpper()}&to={toCurrency.ToUpper()}";

        _logger.LogInformation("Fetching historical exchange rates from {Url}", url);

        try
        {
            var response = await _httpClient.GetFromJsonAsync<FrankfurterHistoryResponse>(url);

            if (response == null || response.Rates == null || response.Rates.Count == 0)
            {
                _logger.LogWarning(
                    "No historical rates returned for {From} to {To} from {Start} to {End}",
                    fromCurrency, toCurrency, start, end);
                return Array.Empty<ExchangeRateDataPoint>();
            }

            var toCurrencyUpper = toCurrency.ToUpper();
            var dataPoints = response.Rates
                .Where(kvp => kvp.Value.ContainsKey(toCurrencyUpper))
                .Select(kvp => new ExchangeRateDataPoint
                {
                    Date = DateOnly.Parse(kvp.Key),
                    Rate = kvp.Value[toCurrencyUpper]
                })
                .OrderBy(dp => dp.Date)
                .ToList();

            return dataPoints.AsReadOnly();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching historical exchange rates from Frankfurter API");
            throw;
        }
    }
}
