using System.Threading.Tasks;

namespace TravelGuideApi.Domain.Interfaces;

/// <summary>
/// Defines service for fetching exchange rate data from external API.
/// </summary>
public interface IExchangeRateApiService
{
    /// <summary>
    /// Gets the exchange rate between two currencies.
    /// </summary>
    /// <param name="fromCurrency">Source currency code (e.g., "USD").</param>
    /// <param name="toCurrency">Target currency code (e.g., "EUR").</param>
    /// <returns>Exchange rate.</returns>
    Task<decimal> GetExchangeRateAsync(string fromCurrency, string toCurrency);
}
