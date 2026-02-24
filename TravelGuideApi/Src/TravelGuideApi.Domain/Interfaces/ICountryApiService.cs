using System.Collections.Generic;
using System.Threading.Tasks;
using TravelGuideApi.Domain.Entities;

namespace TravelGuideApi.Domain.Interfaces;

/// <summary>
/// Defines service for fetching country data from external API.
/// </summary>
public interface ICountryApiService
{
    /// <summary>
    /// Gets all countries with their currency information.
    /// </summary>
    /// <returns>Collection of countries.</returns>
    Task<IEnumerable<CountryEntity>> GetAllCountriesAsync();

    /// <summary>
    /// Gets a country by its ISO 3166-1 alpha-2 code.
    /// </summary>
    /// <param name="code">Country code (e.g., "US", "GB").</param>
    /// <returns>Country if found, null otherwise.</returns>
    Task<CountryEntity?> GetCountryByCodeAsync(string code);
}
