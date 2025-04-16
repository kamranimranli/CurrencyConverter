using CurrencyConverter.Api.Models;

namespace CurrencyConverter.Api.Services;
public interface IExchangeRateService
{
    Task<ExchangeRateResponse> GetLatestRatesAsync(string baseCurrency, CancellationToken cancellationToken = default);
    Task<decimal> ConvertAsync(ConversionRequest request, CancellationToken cancellationToken = default);
    Task<HistoricalRateResponse> GetHistoricalRatesAsync(HistoricalRatesRequest request, CancellationToken cancellationToken = default);
}
