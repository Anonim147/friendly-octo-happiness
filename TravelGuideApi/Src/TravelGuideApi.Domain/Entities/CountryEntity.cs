namespace TravelGuideApi.Domain.Entities;

/// <summary>
/// Represents a country with its currency information.
/// </summary>
public class CountryEntity
{
    /// <summary>
    /// ISO 3166-1 alpha-2 country code (e.g., "US", "GB").
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Common name of the country.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// ISO 4217 currency code (e.g., "USD", "GBP").
    /// </summary>
    public string CurrencyCode { get; set; } = string.Empty;

    /// <summary>
    /// Full name of the currency.
    /// </summary>
    public string CurrencyName { get; set; } = string.Empty;

    /// <summary>
    /// Currency symbol (e.g., "$", "£").
    /// </summary>
    public string CurrencySymbol { get; set; } = string.Empty;

    /// <summary>
    /// URL to the country's flag image.
    /// </summary>
    public string FlagUrl { get; set; } = string.Empty;
}
