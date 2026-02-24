using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TravelGuideApi.Application.Interfaces;
using TravelGuideApi.Domain.Interfaces;
using TravelGuideApi.Domain.Models;
using TravelGuideApi.Domain.Models.ExternalApi;

namespace TravelGuideApi.Infrastructure.Services;

/// <summary>
/// Service for fetching exchange rate data from Open Exchange Rate API.
/// </summary>
public class ExchangeRateApiService : IExchangeRateApiService
{
    private readonly HttpClient _httpClient;
    private readonly IExchangeRateCache _exchangeRateCache;
    private readonly ILogger<ExchangeRateApiService> _logger;
    private readonly string _apiBaseUrl;

    public ExchangeRateApiService(
        HttpClient httpClient,
        IExchangeRateCache exchangeRateCache,
        ILogger<ExchangeRateApiService> logger,
        IOptions<Config> config)
    {
        _httpClient = httpClient;
        _exchangeRateCache = exchangeRateCache;
        _logger = logger;
        _apiBaseUrl = config.Value.ExchangeRateApiBaseUrl;
    }

    public async Task<decimal> GetExchangeRateAsync(string fromCurrency, string toCurrency)
    {
        if (fromCurrency.Equals(toCurrency, StringComparison.OrdinalIgnoreCase))
        {
            return 1m;
        }

        // Try to get from cache first
        var cachedRates = await _exchangeRateCache.GetRatesAsync(fromCurrency);
        if (cachedRates != null && cachedRates.TryGetValue(toCurrency.ToUpper(), out var cachedRate))
        {
            _logger.LogInformation("Returning cached exchange rate for {From} to {To}", fromCurrency, toCurrency);
            return cachedRate;
        }

        try
        {
            var url = $"{_apiBaseUrl}/{fromCurrency.ToUpper()}";
            _logger.LogInformation("Fetching exchange rates from {Url}", url);

            var response = await _httpClient.GetFromJsonAsync<ExchangeRateResponse>(url);

            if (response == null || response.Result != "success")
            {
                _logger.LogWarning("Failed to get exchange rates for {Currency}", fromCurrency);
                throw new InvalidOperationException($"Failed to get exchange rates for {fromCurrency}");
            }

            // Cache the rates
            await _exchangeRateCache.SetRatesAsync(fromCurrency, response.Rates);
            _logger.LogInformation("Cached exchange rates for {Currency}", fromCurrency);

            if (response.Rates.TryGetValue(toCurrency.ToUpper(), out var rate))
            {
                return rate;
            }

            throw new InvalidOperationException($"Exchange rate not found for {toCurrency}");
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            _logger.LogError(ex, "Error fetching exchange rates from API");
            throw;
        }
    }
}
