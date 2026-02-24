using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using TravelGuideApi.Application.Interfaces;

namespace TravelGuideApi.Infrastructure.Services;

/// <summary>
/// Memory cache implementation for exchange rate data.
/// </summary>
public class ExchangeRateCache : IExchangeRateCache
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<ExchangeRateCache> _logger;
    private const string _cacheKeyPrefix = "exchange_rate_";
    private static readonly TimeSpan _cacheExpiration = TimeSpan.FromHours(1);

    public ExchangeRateCache(IMemoryCache cache, ILogger<ExchangeRateCache> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public Task<Dictionary<string, decimal>?> GetRatesAsync(string baseCurrency)
    {
        string cacheKey = $"{_cacheKeyPrefix}{baseCurrency.ToUpper()}";

        if (_cache.TryGetValue(cacheKey, out Dictionary<string, decimal>? cachedRates))
        {
            _logger.LogDebug("Cache hit for exchange rates with base {Currency}", baseCurrency);
            return Task.FromResult(cachedRates);
        }

        _logger.LogDebug("Cache miss for exchange rates with base {Currency}", baseCurrency);
        return Task.FromResult<Dictionary<string, decimal>?>(null);
    }

    public Task SetRatesAsync(string baseCurrency, Dictionary<string, decimal> rates)
    {
        string cacheKey = $"{_cacheKeyPrefix}{baseCurrency.ToUpper()}";
        MemoryCacheEntryOptions cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(_cacheExpiration);

        _cache.Set(cacheKey, rates, cacheOptions);
        _logger.LogInformation(
            "Cached exchange rates for {Currency} with {Count} rates for {Hours} hour(s)",
            baseCurrency,
            rates.Count,
            _cacheExpiration.TotalHours);

        return Task.CompletedTask;
    }
}
