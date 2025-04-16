using CurrencyConverter.Api.Enums;

namespace CurrencyConverter.Api.Providers;

public interface ICurrencyProviderFactory
{
    ICurrencyProvider Create(CurrencyProviderType type);
}
