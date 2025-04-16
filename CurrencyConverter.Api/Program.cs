using AspNetCoreRateLimit;
using CurrencyConverter.Api.Extensions;
using CurrencyConverter.Api.Middlewares;
using FluentValidation.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthentication(builder.Configuration);
builder.Services.AddSwagger();
builder.Services.AddAuthorization();
builder.Services.AddCurrencyConverterServices(builder.Configuration);
builder.Services.AddFluentValidation();
builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddRateLimiting(builder.Configuration);
builder.Services.AddOpenTelemetry(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseIpRateLimiting();
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
public partial class Program { }