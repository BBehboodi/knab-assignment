using System.Text.Json.Serialization;

namespace Knab.Assignment.API.Models.Proxy
{
    public class CryptocurrenciesResponse
    {
        [JsonConstructor]
        public CryptocurrenciesResponse(StatusResponse status, List<CryptocurrencyResponse>? cryptocurrencies)
        {
            Status = status;
            Cryptocurrencies = cryptocurrencies;
            Succeed = Status.ErrorCode == 0;
        }

        [JsonPropertyName("status")]
        public StatusResponse Status { get; }

        [JsonPropertyName("data")]
        public List<CryptocurrencyResponse>? Cryptocurrencies { get; }

        public bool Succeed { get; }
    }
}