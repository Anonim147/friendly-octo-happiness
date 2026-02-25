using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using TravelGuideApi.Domain.Interfaces;
using TravelGuideApi.Domain.Models;

namespace TravelGuideApi.Application.Features.ExchangeRateHistory.Queries;

/// <summary>
/// Handles <see cref="ExchangeRateHistoryQuery"/> queries.
/// </summary>
public class ExchangeRateHistoryQueryHandler(
    IExchangeRateHistoryService exchangeRateHistoryService,
    ILogger<ExchangeRateHistoryQueryHandler> logger)
    : IRequestHandler<ExchangeRateHistoryQuery, IReadOnlyList<ExchangeRateDataPoint>>
{
    private readonly IExchangeRateHistoryService _exchangeRateHistoryService = exchangeRateHistoryService;
    private readonly ILogger<ExchangeRateHistoryQueryHandler> _logger = logger;

    public async Task<IReadOnlyList<ExchangeRateDataPoint>> Handle(
        ExchangeRateHistoryQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Fetching {Days}-day exchange rate history from {From} to {To}",
            request.Days,
            request.FromCurrency,
            request.ToCurrency);

        var endDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var startDate = endDate.AddDays(-request.Days);

        var dataPoints = await _exchangeRateHistoryService.GetHistoricalRatesAsync(
            request.FromCurrency,
            request.ToCurrency,
            startDate,
            endDate);

        return dataPoints;
    }
}
