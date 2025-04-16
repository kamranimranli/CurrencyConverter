namespace CurrencyConverter.Api.Models;

public class ConversionRequest
{
    public string From { get; set; } = default!;
    public string To { get; set; } = default!;
    public decimal Amount { get; set; }
}
