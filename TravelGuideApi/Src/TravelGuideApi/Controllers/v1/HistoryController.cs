using System.Collections.Generic;
using System.Threading.Tasks;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TravelGuideApi.Application.Features.ExchangeRateHistory.Queries;
using TravelGuideApi.Domain.Models;

namespace TravelGuideApi.Controllers.v1;

/// <summary>
/// Controller for historical exchange rate operations.
/// </summary>
[ApiVersion("1.0")]
public class HistoryController(
    ILogger<HistoryController> logger,
    IMediator mediator)
    : BaseApiController(mediator)
{
    private readonly ILogger<HistoryController> _logger = logger;

    /// <summary>
    /// Gets historical exchange rate data for a currency pair.
    /// </summary>
    /// <param name="from">Source currency code (e.g., "USD").</param>
    /// <param name="to">Target currency code (e.g., "EUR").</param>
    /// <param name="days">Number of days of history: 30, 90, or 365.</param>
    /// <returns>List of exchange rate data points ordered by date.</returns>
    [HttpGet("rates")]
    [ProducesResponseType(typeof(IReadOnlyList<ExchangeRateDataPoint>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetHistoricalRates(
        [FromQuery] string from,
        [FromQuery] string to,
        [FromQuery] int days)
    {
        _logger.LogInformation(
            "Getting {Days}-day historical rates from {From} to {To}",
            days, from, to);

        var query = new ExchangeRateHistoryQuery(from, to, days);
        IReadOnlyList<ExchangeRateDataPoint> result = await Mediator.Send(query);
        return Ok(result);
    }
}
