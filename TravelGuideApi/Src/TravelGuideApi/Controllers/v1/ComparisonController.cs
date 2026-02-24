using System.Collections.Generic;
using System.Threading.Tasks;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TravelGuideApi.Application.Features.Country.Queries.GetAll;
using TravelGuideApi.Application.Features.CurrencyComparison.Queries.Compare;
using TravelGuideApi.Domain.Entities;
using TravelGuideApi.Domain.Models;

namespace TravelGuideApi.Controllers.v1;

/// <summary>
/// Controller for country and currency comparison operations.
/// </summary>
[ApiVersion("1.0")]
public class ComparisonController(
    ILogger<ComparisonController> logger,
    IMediator mediator)
    : BaseApiController(mediator)
{
    private readonly ILogger<ComparisonController> _logger = logger;

    /// <summary>
    /// Gets all available countries with their currency information.
    /// </summary>
    /// <returns>Collection of countries.</returns>
    [HttpGet("countries")]
    [ProducesResponseType(typeof(IEnumerable<CountryEntity>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCountries()
    {
        _logger.LogInformation("Getting all countries");
        IEnumerable<CountryEntity> result = await Mediator.Send(new CountryGetAllQuery());
        return Ok(result);
    }

    /// <summary>
    /// Compares currencies between two countries.
    /// </summary>
    /// <param name="request">Currency comparison request with home and destination country codes.</param>
    /// <returns>Currency comparison result.</returns>
    [HttpPost("currency")]
    [ProducesResponseType(typeof(CurrencyComparisonResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CompareCurrency([FromBody] CurrencyCompareQuery request)
    {
        _logger.LogInformation(
            "Comparing currencies: {HomeCountryCode} to {DestinationCountryCode}",
            request.HomeCountryCode,
            request.DestinationCountryCode);

        CurrencyComparisonResult result = await Mediator.Send(request);
        return Ok(result);
    }
}
