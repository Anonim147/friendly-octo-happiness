using System.Collections.Generic;
using System.Threading.Tasks;
using TravelGuideApi.Domain.Entities;

namespace TravelGuideApi.Application.Interfaces;

/// <summary>
/// Defines cache for country data.
/// </summary>
public interface ICountryCache
{
    /// <summary>
    /// Gets all cached countries.
    /// </summary>
    /// <returns>Cached countries or null if not cached.</returns>
    Task<IEnumerable<CountryEntity>?> GetAllAsync();

    /// <summary>
    /// Sets all countries in cache.
    /// </summary>
    /// <param name="countries">Countries to cache.</param>
    Task SetAllAsync(IEnumerable<CountryEntity> countries);

    /// <summary>
    /// Gets a country by code from cache.
    /// </summary>
    /// <param name="code">Country code.</param>
    /// <returns>Cached country or null if not found.</returns>
    Task<CountryEntity?> GetByCodeAsync(string code);
}
