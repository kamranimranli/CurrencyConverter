using CurrencyConverter.Api.Enums;
using CurrencyConverter.Api.Models;
using CurrencyConverter.Api.Providers;
using CurrencyConverter.Api.Services;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;

namespace CurrencyConverter.Tests.Unit;

public class ExchangeRateServiceTests
{
    private readonly Mock<ICurrencyProviderFactory> _mockFactory = new();
    private readonly Mock<ILogger<ExchangeRateService>> _mockLogger = new();
    private readonly IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

    [Fact]
    public async Task GetLatestRatesAsync_ShouldReturnCachedResponse()
    {
        var response = new ExchangeRateResponse
        {
            Base = "EUR",
            Date = DateTime.Today,
            Rates = new Dictionary<string, decimal> { { "USD", 1.1m } }
        };

        var mockProvider = new Mock<ICurrencyProvider>();
        mockProvider
            .Setup(p => p.GetLatestRatesAsync("EUR", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        _mockFactory
            .Setup(f => f.Create(It.IsAny<CurrencyProviderType>()))
            .Returns(mockProvider.Object);

        var service = new ExchangeRateService(_mockFactory.Object, _cache, _mockLogger.Object);

        var result = await service.GetLatestRatesAsync("EUR");

        result.Should().NotBeNull();
        result.Rates.Should().ContainKey("USD");
        result.Base.Should().Be("EUR");
    }

    [Fact]
    public async Task GetHistoricalRatesAsync_ShouldReturnPagedResponse()
    {
        // Arrange
        var request = new HistoricalRatesRequest
        {
            BaseCurrency = "EUR",
            From = new DateTime(2024, 01, 01),
            To = new DateTime(2024, 01, 05),
            Page = 2,
            PageSize = 2
        };

        var fullRates = new Dictionary<string, Dictionary<string, decimal>>
        {
            { "2024-01-01", new() { { "USD", 1.1m }, { "GBP", 0.87m } } },
            { "2024-01-02", new() { { "USD", 1.12m }, { "GBP", 0.88m } } },
            { "2024-01-03", new() { { "USD", 1.13m }, { "GBP", 0.89m } } },
            { "2024-01-04", new() { { "USD", 1.14m }, { "GBP", 0.90m } } },
            { "2024-01-05", new() { { "USD", 1.15m }, { "GBP", 0.91m } } }
        };

        var historicalResponse = new HistoricalRateResponse
        {
            BaseCurrency = request.BaseCurrency,
            StartDate = request.From,
            EndDate = request.To,
            Rates = fullRates
        };

        var mockProvider = new Mock<ICurrencyProvider>();
        mockProvider
            .Setup(p => p.GetHistoricalRatesAsync(request.BaseCurrency, request.From, request.To, It.IsAny<CancellationToken>()))
            .ReturnsAsync(historicalResponse);

        _mockFactory
            .Setup(f => f.Create(It.IsAny<CurrencyProviderType>()))
            .Returns(mockProvider.Object);

        var service = new ExchangeRateService(_mockFactory.Object, _cache, _mockLogger.Object);

        // Act
        var result = await service.GetHistoricalRatesAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.BaseCurrency.Should().Be(request.BaseCurrency);
        result.Rates.Should().HaveCount(2);
        result.Rates.Keys.Should().Contain("2024-01-03");
        result.Rates.Keys.Should().Contain("2024-01-04");
    }
}
