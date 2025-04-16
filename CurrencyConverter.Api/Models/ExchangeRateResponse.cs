namespace CurrencyConverter.Api.Models;

public class ExchangeRateResponse
{
    public string Base { get; set; } = default!;
    public DateTime Date { get; set; }
    public Dictionary<string, decimal> Rates { get; set; } = new();
}
