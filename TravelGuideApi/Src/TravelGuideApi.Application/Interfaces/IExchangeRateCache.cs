using System.Collections.Generic;
using System.Threading.Tasks;

namespace TravelGuideApi.Application.Interfaces;

/// <summary>
/// Defines cache for exchange rate data.
/// </summary>
public interface IExchangeRateCache
{
    /// <summary>
    /// Gets cached exchange rates for a base currency.
    /// </summary>
    /// <param name="baseCurrency">Base currency code.</param>
    /// <returns>Dictionary of rates or null if not cached.</returns>
    Task<Dictionary<string, decimal>?> GetRatesAsync(string baseCurrency);

    /// <summary>
    /// Sets exchange rates in cache for a base currency.
    /// </summary>
    /// <param name="baseCurrency">Base currency code.</param>
    /// <param name="rates">Exchange rates.</param>
    Task SetRatesAsync(string baseCurrency, Dictionary<string, decimal> rates);
}
