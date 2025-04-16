using AspNetCoreRateLimit;
using CurrencyConverter.Api.Helpers;
using CurrencyConverter.Api.Providers;
using CurrencyConverter.Api.Services;
using CurrencyConverter.Api.Settings;
using CurrencyConverter.Api.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Text;

namespace CurrencyConverter.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<JwtOptions>(config.GetSection("Jwt"));
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(config["Jwt:Key"]!))
                };
            });
        return services;
    }

    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Currency Converter API", Version = "v1" });

            var jwtSecurityScheme = new OpenApiSecurityScheme
            {
                Scheme = "bearer",
                BearerFormat = "JWT",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                Reference = new OpenApiReference
                {
                    Id = JwtBearerDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            };

            options.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { jwtSecurityScheme, Array.Empty<string>() }
            });
        });
        return services;
    }

    public static IServiceCollection AddCurrencyConverterServices(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<FrankfurterOptions>(config.GetSection("CurrencyProviders:Frankfurter"));
        services.AddHttpClient<FrankfurterCurrencyProvider>()
                .AddPolicyHandler(PolicyHelper.GetRetryPolicy())
                .AddPolicyHandler(PolicyHelper.GetCircuitBreakerPolicy());

        services.AddScoped<ICurrencyProviderFactory, CurrencyProviderFactory>();
        services.AddScoped<IExchangeRateService, ExchangeRateService>();

        return services;
    }

    public static IServiceCollection AddFluentValidation(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation()
                .AddFluentValidationClientsideAdapters()
                .AddValidatorsFromAssemblyContaining<AuthRequestValidator>(lifetime: ServiceLifetime.Scoped)
                .AddValidatorsFromAssemblyContaining<ConversionRequestValidator>(lifetime: ServiceLifetime.Scoped)
                .AddValidatorsFromAssemblyContaining<HistoricalRatesRequestValidator>(lifetime: ServiceLifetime.Scoped);

        return services;
    }

    public static IServiceCollection AddRateLimiting(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<IpRateLimitOptions>(config.GetSection("IpRateLimiting"));

        services.AddInMemoryRateLimiting();

        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

        return services;
    }

    public static IServiceCollection AddOpenTelemetry(this IServiceCollection services, IConfiguration config)
    {
        services.AddOpenTelemetry()
            .WithTracing(builder =>
            {
                builder
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddJaegerExporter(o =>
                    {
                        o.AgentHost = config.GetSection("Jaeger:Host").Value;
                        o.AgentPort = Convert.ToInt32(config.GetSection("Jaeger:Port").Value);
                    })
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("CurrencyConverter.Api"));
            });
        return services;
    }
}
