using System.Collections.Generic;
using System.Threading.Tasks;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TravelGuideApi.Application.Features.Budget.Queries.Calculate;
using TravelGuideApi.Domain.Models;

namespace TravelGuideApi.Controllers.v1;

/// <summary>
/// Controller for trip budget calculation operations.
/// </summary>
[ApiVersion("1.0")]
public class BudgetController(
    ILogger<BudgetController> logger,
    IMediator mediator)
    : BaseApiController(mediator)
{
    private readonly ILogger<BudgetController> _logger = logger;

    /// <summary>
    /// Calculates a trip budget across one or more destination countries.
    /// </summary>
    /// <param name="request">Budget calculation request with home currency, daily budget, trip days, and destination countries.</param>
    /// <returns>List of budget calculation results per destination country.</returns>
    [HttpPost("calculate")]
    [ProducesResponseType(typeof(List<BudgetCalculationResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Calculate([FromBody] BudgetCalculateQuery request)
    {
        _logger.LogInformation(
            "Calculating trip budget: {HomeCurrency}, {DailyBudget}/day, {TripDays} days, {DestinationCount} destination(s)",
            request.HomeCurrency,
            request.DailyBudget,
            request.TripDays,
            request.DestinationCountries.Count);

        List<BudgetCalculationResult> result = await Mediator.Send(request);
        return Ok(result);
    }
}
