namespace CurrencyConverter.Api.Models
{
    public class HistoricalRatesRequest : Pagination
    {
        public string BaseCurrency { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}
