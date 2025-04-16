using System.Text.Json.Serialization;

namespace CurrencyConverter.Api.Models;

public class HistoricalRateResponse
{
    [JsonPropertyName("base")]
    public string BaseCurrency { get; set; } = default!;

    [JsonPropertyName("start_date")]
    public DateTime StartDate { get; set; }

    [JsonPropertyName("end_date")]
    public DateTime EndDate { get; set; }

    [JsonPropertyName("rates")]
    public Dictionary<string, Dictionary<string, decimal>> Rates { get; set; } = new();
}