using System.Collections.Generic;
using MediatR;
using TravelGuideApi.Domain.Models;

namespace TravelGuideApi.Application.Features.ExchangeRateHistory.Queries;

/// <summary>
/// Query to retrieve historical exchange rate data for a currency pair.
/// </summary>
/// <param name="FromCurrency">The source currency code (e.g., "USD").</param>
/// <param name="ToCurrency">The target currency code (e.g., "EUR").</param>
/// <param name="Days">Number of days of history to retrieve (30, 90, or 365).</param>
public record ExchangeRateHistoryQuery(
    string FromCurrency,
    string ToCurrency,
    int Days)
    : IRequest<IReadOnlyList<ExchangeRateDataPoint>>;
