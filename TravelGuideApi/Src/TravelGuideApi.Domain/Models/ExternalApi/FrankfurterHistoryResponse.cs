using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TravelGuideApi.Domain.Models.ExternalApi;

/// <summary>
/// Response model from the Frankfurter historical exchange rates API.
/// </summary>
public class FrankfurterHistoryResponse
{
    /// <summary>
    /// Base currency code.
    /// </summary>
    [JsonPropertyName("base")]
    public string Base { get; set; } = string.Empty;

    /// <summary>
    /// Start date of the range.
    /// </summary>
    [JsonPropertyName("start_date")]
    public string StartDate { get; set; } = string.Empty;

    /// <summary>
    /// End date of the range.
    /// </summary>
    [JsonPropertyName("end_date")]
    public string EndDate { get; set; } = string.Empty;

    /// <summary>
    /// Dictionary keyed by date string (yyyy-MM-dd), value is a dictionary of currency -> rate.
    /// </summary>
    [JsonPropertyName("rates")]
    public Dictionary<string, Dictionary<string, decimal>> Rates { get; set; } = new();
}
