using CurrencyConverter.Api.Models;
using CurrencyConverter.Api.Settings;
using Microsoft.Extensions.Options;

namespace CurrencyConverter.Api.Providers;

public class FrankfurterCurrencyProvider : ICurrencyProvider
{
    private readonly HttpClient _httpClient;

    public FrankfurterCurrencyProvider(HttpClient httpClient, IOptions<FrankfurterOptions> options)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(options.Value.BaseUrl);
    }

    public async Task<ExchangeRateResponse> GetLatestRatesAsync(string baseCurrency, string[]? symbols = null, CancellationToken cancellationToken = default)
    {
        var symbolQuery = symbols != null ? $"&symbols={string.Join(",", symbols)}" : "";
        var url = $"latest?base={baseCurrency}{symbolQuery}";

        var response = await _httpClient.GetFromJsonAsync<ExchangeRateResponse>(url, cancellationToken);
        return response!;
    }

    public async Task<HistoricalRateResponse> GetHistoricalRatesAsync(string baseCurrency, DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        var url = $"{from:yyyy-MM-dd}..{to:yyyy-MM-dd}?base={baseCurrency}";
        var response = await _httpClient.GetFromJsonAsync<HistoricalRateResponse>(url, cancellationToken);
        return response!;
    }
}