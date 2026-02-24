namespace TravelGuideApi.Domain.Models;

/// <summary>
/// Result of comparing currencies between two countries.
/// </summary>
public class CurrencyComparisonResult
{
    /// <summary>
    /// Name of the home country.
    /// </summary>
    public string HomeCountry { get; set; } = string.Empty;

    /// <summary>
    /// Currency code of the home country.
    /// </summary>
    public string HomeCurrency { get; set; } = string.Empty;

    /// <summary>
    /// Name of the destination country.
    /// </summary>
    public string DestinationCountry { get; set; } = string.Empty;

    /// <summary>
    /// Currency code of the destination country.
    /// </summary>
    public string DestinationCurrency { get; set; } = string.Empty;

    /// <summary>
    /// Exchange rate from home currency to destination currency.
    /// </summary>
    public decimal ExchangeRate { get; set; }

    /// <summary>
    /// Hint about currency strength comparison.
    /// </summary>
    public string StrengthHint { get; set; } = string.Empty;
}
