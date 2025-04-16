using CurrencyConverter.Api.Models;
using CurrencyConverter.Api.Providers;

namespace CurrencyConverter.Tests.Mocks;

public class MockCurrencyProvider : ICurrencyProvider
{
    public Task<ExchangeRateResponse> GetLatestRatesAsync(string baseCurrency, string[]? symbols = null, CancellationToken cancellationToken = default)
    {
        var rates = new Dictionary<string, decimal>
        {
            { "USD", 1.1m },
            { "GBP", 0.88m },
            { "EUR", 1.0m }
        };

        if (symbols != null && symbols.Length > 0)
        {
            rates = rates
                .Where(kvp => symbols.Contains(kvp.Key))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        var response = new ExchangeRateResponse
        {
            Base = baseCurrency,
            Date = DateTime.UtcNow.Date,
            Rates = rates
        };

        return Task.FromResult(response);
    }

    public Task<HistoricalRateResponse> GetHistoricalRatesAsync(string baseCurrency, DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        var historicalRates = new Dictionary<string, Dictionary<string, decimal>>();

        for (var date = from.Date; date <= to.Date; date = date.AddDays(1))
        {
            var dayKey = date.ToString("yyyy-MM-dd");

            historicalRates[dayKey] = new Dictionary<string, decimal>
            {
                { "USD", 1.1m + (decimal)(date.Day % 3) / 100 },
                { "GBP", 0.87m + (decimal)(date.Day % 2) / 100 }
            };
        }

        var response = new HistoricalRateResponse
        {
            BaseCurrency = baseCurrency,
            StartDate = from,
            EndDate = to,
            Rates = historicalRates
        };

        return Task.FromResult(response);
    }
}
