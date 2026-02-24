using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TravelGuideApi.Domain.Entities;
using TravelGuideApi.Domain.Interfaces;
using TravelGuideApi.Domain.Models;
using TravelGuideApi.Domain.Models.ExternalApi;

namespace TravelGuideApi.Infrastructure.Services;

/// <summary>
/// Service for fetching country data from REST Countries API.
/// </summary>
public class CountryApiService : ICountryApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CountryApiService> _logger;
    private readonly string _apiUrl;

    public CountryApiService(HttpClient httpClient, ILogger<CountryApiService> logger, IOptions<Config> config)
    {
        _httpClient = httpClient;
        _logger = logger;
        _apiUrl = config.Value.CountryApiUrl;
    }

    public async Task<IEnumerable<CountryEntity>> GetAllCountriesAsync()
    {
        try
        {
            _logger.LogInformation("Fetching countries from REST Countries API");
            var response = await _httpClient.GetFromJsonAsync<List<RestCountryResponse>>(_apiUrl);

            if (response == null)
            {
                _logger.LogWarning("No data received from REST Countries API");
                return Enumerable.Empty<CountryEntity>();
            }

            var countries = response
                .Where(c => c.Currencies != null && c.Currencies.Any())
                .Select(MapToCountryEntity)
                .OrderBy(c => c.Name)
                .ToList();

            _logger.LogInformation("Fetched {Count} countries from REST Countries API", countries.Count);
            return countries;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching countries from REST Countries API");
            throw;
        }
    }

    public async Task<CountryEntity?> GetCountryByCodeAsync(string code)
    {
        var countries = await GetAllCountriesAsync();
        return countries.FirstOrDefault(c => c.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
    }

    private static CountryEntity MapToCountryEntity(RestCountryResponse response)
    {
        var currency = response.Currencies!.First();
        return new CountryEntity
        {
            Code = response.Cca2,
            Name = response.Name.Common,
            CurrencyCode = currency.Key,
            CurrencyName = currency.Value.Name,
            CurrencySymbol = currency.Value.Symbol ?? "",
            FlagUrl = response.Flags?.Png ?? ""
        };
    }
}
