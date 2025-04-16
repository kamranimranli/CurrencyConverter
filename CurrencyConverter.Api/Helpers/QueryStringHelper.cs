namespace CurrencyConverter.Api.Helpers;

public static class QueryStringHelper
{
    public static string ToQueryString(object obj)
    {
        var props = from p in obj.GetType().GetProperties()
                    let value = p.GetValue(obj, null)
                    where value != null
                    select $"{Uri.EscapeDataString(p.Name)}={Uri.EscapeDataString(value.ToString()!)}";

        return string.Join("&", props);
    }
}
