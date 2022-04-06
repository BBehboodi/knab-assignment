using Knab.Assignment.API.Models.Dto;
using System.Text.Json.Serialization;

namespace Knab.Assignment.API.Models.Proxy
{
    public class CryptocurrencyQuoteResponse : CryptocurrencyResponse
    {
        [JsonConstructor]
        public CryptocurrencyQuoteResponse(int id, string name, string symbol, Dictionary<string, QuoteInfoResponse> quotes)
            : base(id, name, symbol)
        {
            Quotes = quotes;
        }

        [JsonPropertyName("quote")]
        public Dictionary<string, QuoteInfoResponse> Quotes { get; }

        public new CryptocurrencyQuoteDto ToDto()
        {
            var quotes = Quotes.ToDictionary(k => k.Key, v => v.Value.Price);
            return new CryptocurrencyQuoteDto(Name, quotes);
        }
    }
}