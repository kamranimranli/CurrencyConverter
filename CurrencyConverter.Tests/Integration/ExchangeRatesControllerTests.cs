using CurrencyConverter.Api.Helpers;
using CurrencyConverter.Api.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace CurrencyConverter.Tests.Integration;

public class ExchangeRatesControllerTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task LatestRates_WithValidToken_ShouldReturnOk()
    {
        // Arrange
        var token = await GetJwtTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/exchangerates/latest?baseCurrency=USD");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/json");
    }

    [Fact]
    public async Task Convert_WithValidPayload_ShouldReturnResult()
    {
        // Arrange
        var token = await GetJwtTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var payload = new
        {
            From = "EUR",
            To = "USD",
            Amount = 100
        };

        var content = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json"
        );

        // Act
        var response = await _client.PostAsync("/api/exchangerates/convert", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/json");

        var json = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        root.GetProperty("amount").GetDecimal().Should().Be(100);
        root.GetProperty("from").GetString().Should().Be("EUR");
        root.GetProperty("to").GetString().Should().Be("USD");
        root.TryGetProperty("result", out var resultProp).Should().BeTrue("Expected 'result' property in response");
        resultProp.GetDecimal().Should().BeGreaterThan(0);
    }


    [Fact]
    public async Task History_WithValidQuery_ShouldReturnOk()
    {
        var token = await GetJwtTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var query = new HistoricalRatesRequest
        {
            BaseCurrency = "EUR",
            From = new DateTime(2024, 01, 01),
            To = new DateTime(2024, 01, 05),
            Page = 1,
            PageSize = 2
        };

        var queryString = QueryStringHelper.ToQueryString(query);
        var response = await _client.GetAsync($"/api/exchangerates/history?{queryString}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task LatestRates_WithoutToken_ShouldReturn401()
    {
        var response = await _client.GetAsync("/api/exchangerates/latest?baseCurrency=EUR");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }

    private async Task<string> GetJwtTokenAsync()
    {
        var payload = new
        {
            userId = "test-user",
            role = "Admin",
            expireMinutes = 60
        };

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/auth/token", content);
        var json = await response.Content.ReadAsStringAsync();

        var doc = JsonDocument.Parse(json);
        return doc.RootElement.GetProperty("access_token").GetString()!;
    }
}
