using Knab.Assignment.API.Models.Dto;
using System.Text.Json.Serialization;

namespace Knab.Assignment.API.Models.Proxy
{
    public class CryptocurrencyResponse
    {
        public CryptocurrencyResponse(int id, string name, string symbol)
        {
            Id = id;
            Name = name;
            Symbol = symbol;
        }

        public int Id { get; }

        [JsonPropertyName("name")]
        public string Name { get; }

        [JsonPropertyName("symbol")]
        public string Symbol { get; }

        public CryptocurrencyDto ToDto()
        {
            return new CryptocurrencyDto(Id, Name, Symbol);
        }
    }
}
