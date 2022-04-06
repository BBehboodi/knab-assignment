namespace Knab.Assignment.API.Models.Dto
{
    public record class CryptocurrencyQuoteDto
    {
        public CryptocurrencyQuoteDto(string cryptocurrency, Dictionary<string, decimal> quotes)
        {
            Cryptocurrency = cryptocurrency;
            Quotes = quotes;
        }

        public string Cryptocurrency { get; }

        public Dictionary<string, decimal> Quotes { get; }

        public CryptocurrencyQuote ToModel()
        {
            return new CryptocurrencyQuote(Cryptocurrency, Quotes);
        }
    }
}