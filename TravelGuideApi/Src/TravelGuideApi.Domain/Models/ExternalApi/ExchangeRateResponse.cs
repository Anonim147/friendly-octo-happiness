using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TravelGuideApi.Domain.Models.ExternalApi;

/// <summary>
/// Response model from Exchange Rate API.
/// </summary>
public class ExchangeRateResponse
{
    [JsonPropertyName("result")]
    public string Result { get; set; } = string.Empty;

    [JsonPropertyName("base_code")]
    public string BaseCode { get; set; } = string.Empty;

    [JsonPropertyName("time_last_update_utc")]
    public string TimeLastUpdateUtc { get; set; } = string.Empty;

    [JsonPropertyName("rates")]
    public Dictionary<string, decimal> Rates { get; set; } = new();
}
