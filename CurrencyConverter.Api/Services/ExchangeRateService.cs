using CurrencyConverter.Api.Enums;
using CurrencyConverter.Api.Models;
using CurrencyConverter.Api.Providers;
using Microsoft.Extensions.Caching.Memory;

namespace CurrencyConverter.Api.Services;

public class ExchangeRateService : IExchangeRateService
{
    private readonly ICurrencyProvider _provider;
    private readonly IMemoryCache _cache;
    private readonly ILogger<ExchangeRateService> _logger;

    public ExchangeRateService(
        ICurrencyProviderFactory providerFactory,
        IMemoryCache cache,
        ILogger<ExchangeRateService> logger)
    {
        _provider = providerFactory.Create(CurrencyProviderType.Frankfurter);
        _cache = cache;
        _logger = logger;
    }

    public async Task<ExchangeRateResponse> GetLatestRatesAsync(string baseCurrency, CancellationToken cancellationToken = default)
    {
        const int CacheDurationMinutes = 30;
        var cacheKey = $"latest_{baseCurrency}";

        _logger.LogInformation("Fetching latest exchange rates for base currency: {Base}", baseCurrency);

        return await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheDurationMinutes);

            var rates = await _provider.GetLatestRatesAsync(baseCurrency, cancellationToken: cancellationToken);

            _logger.LogInformation("Cached exchange rates for {Base}", baseCurrency);

            return rates;
        });
    }

    public async Task<decimal> ConvertAsync(ConversionRequest request, CancellationToken cancellationToken = default)
    {
        var latest = await _provider.GetLatestRatesAsync(request.From, new[] { request.To }, cancellationToken);

        if (!latest.Rates.TryGetValue(request.To, out var rate))
        {
            _logger.LogError("Rate not found for conversion: {From} → {To}", request.From, request.To);
            throw new KeyNotFoundException($"Rate not found for {request.To}");
        }

        var result = Math.Round(request.Amount * rate, 2);
        _logger.LogInformation("Converted {Amount} {From} to {To}: {Result}", request.Amount, request.From, request.To, result);
        return result;
    }


    public async Task<HistoricalRateResponse> GetHistoricalRatesAsync(HistoricalRatesRequest request, CancellationToken cancellationToken = default)
    {
        var fullData = await _provider.GetHistoricalRatesAsync(request.BaseCurrency, request.From, request.To, cancellationToken);

        var pagedRates = fullData.Rates
            .OrderBy(kvp => kvp.Key)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToDictionary(k => k.Key, v => v.Value);

        _logger.LogInformation("Historical rates fetched: {Count} days", pagedRates.Count);

        return new HistoricalRateResponse
        {
            BaseCurrency = fullData.BaseCurrency,
            StartDate = fullData.StartDate,
            EndDate = fullData.EndDate,
            Rates = pagedRates
        };
    }
}