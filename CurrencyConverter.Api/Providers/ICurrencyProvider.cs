using CurrencyConverter.Api.Models;

namespace CurrencyConverter.Api.Providers;
public interface ICurrencyProvider
{
    Task<ExchangeRateResponse> GetLatestRatesAsync(string baseCurrency, string[]? symbols = null, CancellationToken cancellationToken = default);
    Task<HistoricalRateResponse> GetHistoricalRatesAsync(string baseCurrency, DateTime from, DateTime to, CancellationToken cancellationToken = default);
}
