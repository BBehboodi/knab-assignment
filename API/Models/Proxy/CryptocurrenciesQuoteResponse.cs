using System.Text.Json.Serialization;

namespace Knab.Assignment.API.Models.Proxy
{
    public class CryptocurrenciesQuoteResponse
    {
        [JsonConstructor]
        public CryptocurrenciesQuoteResponse(StatusResponse status, Dictionary<string, List<CryptocurrencyQuoteResponse>>? cryptocurrenciesQuotes)
        {
            Status = status;
            CryptocurrenciesQuotes = cryptocurrenciesQuotes;
            Succeed = status.ErrorCode == 0;
        }

        [JsonPropertyName("status")]
        public StatusResponse Status { get; }

        [JsonPropertyName("data")]
        public Dictionary<string, List<CryptocurrencyQuoteResponse>>? CryptocurrenciesQuotes { get; }

        public bool Succeed { get; }

        public List<QuoteResponse> ToQuoteResponse()
        {
            if (CryptocurrenciesQuotes is null)
                throw new InvalidOperationException($"{nameof(CryptocurrenciesQuotes)} could not be null");

            return CryptocurrenciesQuotes.First()
                .Value.First()
                .Quotes.Select(x => new QuoteResponse(x.Key, x.Value.Price))
                .ToList();
        }
    }
}