using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TravelGuideApi.Domain.Models;

namespace TravelGuideApi.Domain.Interfaces;

/// <summary>
/// Defines service for fetching historical exchange rate data.
/// </summary>
public interface IExchangeRateHistoryService
{
    /// <summary>
    /// Gets historical exchange rates between two currencies for a date range.
    /// </summary>
    /// <param name="fromCurrency">Source currency code (e.g., "USD").</param>
    /// <param name="toCurrency">Target currency code (e.g., "EUR").</param>
    /// <param name="startDate">Start date of the range.</param>
    /// <param name="endDate">End date of the range.</param>
    /// <returns>List of exchange rate data points ordered by date.</returns>
    Task<IReadOnlyList<ExchangeRateDataPoint>> GetHistoricalRatesAsync(
        string fromCurrency,
        string toCurrency,
        DateOnly startDate,
        DateOnly endDate);
}
