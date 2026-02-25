using System;

namespace TravelGuideApi.Domain.Models;

/// <summary>
/// Represents a single historical exchange rate data point.
/// </summary>
public class ExchangeRateDataPoint
{
    /// <summary>
    /// The date of the exchange rate.
    /// </summary>
    public DateOnly Date { get; set; }

    /// <summary>
    /// The exchange rate value on this date.
    /// </summary>
    public decimal Rate { get; set; }
}
