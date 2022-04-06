namespace Knab.Assignment.API.Models
{
    public class CryptocurrencyQuote
    {
        public CryptocurrencyQuote(string cryptocurrency, Dictionary<string, decimal> quotes)
        {
            Cryptocurrency = cryptocurrency;
            Quotes = quotes;
        }

        /// <summary>
        /// The name of the cryptocurrency
        /// </summary>
        public string Cryptocurrency { get; }

        /// <summary>
        /// The quote of the currencies
        /// </summary>
        public Dictionary<string, decimal> Quotes { get; }
    }
}