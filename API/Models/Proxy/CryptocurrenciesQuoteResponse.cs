using Knab.Assignment.API.Models.Dto;
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
    }
}