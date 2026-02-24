using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using TravelGuideApi.Application.Interfaces;
using TravelGuideApi.Domain.Entities;

namespace TravelGuideApi.Infrastructure.Services;

/// <summary>
/// Memory cache implementation for country data.
/// </summary>
public class CountryCache : ICountryCache
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<CountryCache> _logger;
    private const string _cacheKey = "countries";
    private static readonly TimeSpan _cacheExpiration = TimeSpan.FromHours(24);

    public CountryCache(IMemoryCache cache, ILogger<CountryCache> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public Task<IEnumerable<CountryEntity>?> GetAllAsync()
    {
        if (_cache.TryGetValue(_cacheKey, out List<CountryEntity>? cachedCountries))
        {
            _logger.LogDebug("Cache hit for countries");
            return Task.FromResult<IEnumerable<CountryEntity>?>(cachedCountries);
        }

        _logger.LogDebug("Cache miss for countries");
        return Task.FromResult<IEnumerable<CountryEntity>?>(null);
    }

    public Task SetAllAsync(IEnumerable<CountryEntity> countries)
    {
        var countryList = countries.ToList();
        MemoryCacheEntryOptions cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(_cacheExpiration);

        _cache.Set(_cacheKey, countryList, cacheOptions);
        _logger.LogInformation("Cached {Count} countries for {Hours} hours", countryList.Count, _cacheExpiration.TotalHours);

        return Task.CompletedTask;
    }

    public async Task<CountryEntity?> GetByCodeAsync(string code)
    {
        IEnumerable<CountryEntity>? countries = await GetAllAsync();
        return countries?.FirstOrDefault(c => c.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
    }
}
