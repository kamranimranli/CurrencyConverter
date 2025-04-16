using CurrencyConverter.Api.Models;
using CurrencyConverter.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyConverter.Api.Controllers;

[ApiController]
[Route("api/exchange-rates")]
[Authorize(Roles = "Admin,User")]
public class ExchangeRatesController : ControllerBase
{
    private readonly IExchangeRateService _exchangeRateService;

    public ExchangeRatesController(
        IExchangeRateService exchangeRateService)
    {
        _exchangeRateService = exchangeRateService;
    }

    /// <summary>
    /// Get the latest exchange rates for a given base currency.
    /// </summary>
    [HttpGet("latest")]
    public async Task<IActionResult> GetLatest(string baseCurrency, CancellationToken cancellationToken)
    {
        var result = await _exchangeRateService.GetLatestRatesAsync(baseCurrency, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Convert an amount between two currencies.
    /// </summary>
    [HttpPost("convert")]
    public async Task<IActionResult> Convert([FromBody] ConversionRequest request, CancellationToken cancellationToken)
    {
        var result = await _exchangeRateService.ConvertAsync(request, cancellationToken);
        return Ok(new
        {
            request.Amount,
            request.From,
            request.To,
            Result = result
        });
    }

    /// <summary>
    /// Get historical exchange rates between two dates, with pagination.
    /// </summary>
    [HttpGet("history")]
    public async Task<IActionResult> GetHistoricalRates([FromQuery] HistoricalRatesRequest request, CancellationToken cancellationToken)
    {
        var result = await _exchangeRateService.GetHistoricalRatesAsync(request, cancellationToken);
        return Ok(result);
    }
}
