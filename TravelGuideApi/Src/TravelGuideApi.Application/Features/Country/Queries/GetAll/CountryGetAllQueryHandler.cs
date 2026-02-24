using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using TravelGuideApi.Application.Interfaces;
using TravelGuideApi.Domain.Entities;
using TravelGuideApi.Domain.Interfaces;

namespace TravelGuideApi.Application.Features.Country.Queries.GetAll;

/// <summary>
/// Handles <see cref="CountryGetAllQuery"/> queries.
/// </summary>
public class CountryGetAllQueryHandler(
    ICountryApiService countryApiService,
    ICountryCache countryCache,
    ILogger<CountryGetAllQueryHandler> logger)
    : IRequestHandler<CountryGetAllQuery, IEnumerable<CountryEntity>>
{
    private readonly ICountryApiService _countryApiService = countryApiService;
    private readonly ICountryCache _countryCache = countryCache;
    private readonly ILogger<CountryGetAllQueryHandler> _logger = logger;

    public async Task<IEnumerable<CountryEntity>> Handle(
        CountryGetAllQuery request,
        CancellationToken cancellationToken)
    {
        // Try to get from cache first
        IEnumerable<CountryEntity>? cachedCountries = await _countryCache.GetAllAsync();
        if (cachedCountries != null)
        {
            _logger.LogInformation("Returning cached countries data");
            return cachedCountries;
        }

        // Fetch from API
        _logger.LogInformation("Fetching countries from external API");
        IEnumerable<CountryEntity> countries = await _countryApiService.GetAllCountriesAsync();

        // Cache the results
        await _countryCache.SetAllAsync(countries);

        return countries;
    }
}
