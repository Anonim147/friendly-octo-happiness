namespace TravelGuideApi.Domain.Models;

/// <summary>
/// Defines application strongly-typed config.
/// </summary>
/// <param name="PingValue">Value returned by Ping endpoint.</param>
/// <param name="CountryApiUrl">URL for the REST Countries API.</param>
/// <param name="ExchangeRateApiBaseUrl">Base URL for the Exchange Rate API.</param>
public record Config(string PingValue, string CountryApiUrl, string ExchangeRateApiBaseUrl)
{
    public Config() : this(string.Empty, string.Empty, string.Empty)
    {
    }
}
