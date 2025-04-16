namespace CurrencyConverter.Api.Settings;

public class JwtOptions
{
    public string Key { get; set; } = string.Empty;
    public int ExpireMinutes { get; set; } = 60;
}
