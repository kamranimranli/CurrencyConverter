using CurrencyConverter.Api.Enums;

namespace CurrencyConverter.Api.Providers;

public class CurrencyProviderFactory : ICurrencyProviderFactory
{
    private readonly IServiceProvider _serviceProvider;

    public CurrencyProviderFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ICurrencyProvider Create(CurrencyProviderType type)
    {
        return type switch
        {
            CurrencyProviderType.Frankfurter => _serviceProvider.GetRequiredService<FrankfurterCurrencyProvider>(),
            _ => throw new NotSupportedException($"Currency provider '{type}' is not supported.")
        };
    }
}