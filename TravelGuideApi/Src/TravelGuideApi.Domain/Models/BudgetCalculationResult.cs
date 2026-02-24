namespace TravelGuideApi.Domain.Models;

/// <summary>
/// Result of calculating a trip budget for a destination country.
/// </summary>
public class BudgetCalculationResult
{
    /// <summary>
    /// ISO 3166-1 alpha-2 code of the destination country.
    /// </summary>
    public string CountryCode { get; set; } = string.Empty;

    /// <summary>
    /// Common name of the destination country.
    /// </summary>
    public string CountryName { get; set; } = string.Empty;

    /// <summary>
    /// ISO 4217 currency code of the destination country.
    /// </summary>
    public string DestinationCurrency { get; set; } = string.Empty;

    /// <summary>
    /// Exchange rate from home currency to destination currency.
    /// </summary>
    public decimal ExchangeRate { get; set; }

    /// <summary>
    /// Daily budget in the home currency.
    /// </summary>
    public decimal DailyBudgetHome { get; set; }

    /// <summary>
    /// Number of days for the trip.
    /// </summary>
    public int TripDays { get; set; }

    /// <summary>
    /// Total trip budget in the home currency (DailyBudgetHome * TripDays).
    /// </summary>
    public decimal TotalBudgetHome { get; set; }

    /// <summary>
    /// Total trip budget converted to the destination currency.
    /// </summary>
    public decimal TotalBudgetLocal { get; set; }

    /// <summary>
    /// Daily budget converted to the destination currency.
    /// </summary>
    public decimal DailyLocalAmount { get; set; }

    /// <summary>
    /// Indicates the freshness of the exchange rate data (e.g., "Live").
    /// </summary>
    public string DataFreshness { get; set; } = string.Empty;

    /// <summary>
    /// UTC timestamp when the exchange rate was retrieved.
    /// </summary>
    public DateTime RateTimestamp { get; set; }
}
