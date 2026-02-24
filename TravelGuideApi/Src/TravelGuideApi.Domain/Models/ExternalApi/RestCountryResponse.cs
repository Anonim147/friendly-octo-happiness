using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TravelGuideApi.Domain.Models.ExternalApi;

/// <summary>
/// Response model from REST Countries API.
/// </summary>
public class RestCountryResponse
{
    [JsonPropertyName("name")]
    public CountryName Name { get; set; } = new();

    [JsonPropertyName("cca2")]
    public string Cca2 { get; set; } = string.Empty;

    [JsonPropertyName("currencies")]
    public Dictionary<string, CurrencyInfo>? Currencies { get; set; }

    [JsonPropertyName("flags")]
    public FlagInfo? Flags { get; set; }
}

/// <summary>
/// Country name information.
/// </summary>
public class CountryName
{
    [JsonPropertyName("common")]
    public string Common { get; set; } = string.Empty;

    [JsonPropertyName("official")]
    public string Official { get; set; } = string.Empty;
}

/// <summary>
/// Currency information.
/// </summary>
public class CurrencyInfo
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("symbol")]
    public string Symbol { get; set; } = string.Empty;
}

/// <summary>
/// Flag image URLs.
/// </summary>
public class FlagInfo
{
    [JsonPropertyName("png")]
    public string Png { get; set; } = string.Empty;

    [JsonPropertyName("svg")]
    public string Svg { get; set; } = string.Empty;
}
