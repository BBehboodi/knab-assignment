using System.Text.Json.Serialization;

namespace Knab.Assignment.API.Models
{
    public class Cryptocurrency
    {
        public Cryptocurrency(int id, string name, string symbol)
        {
            Id = id;
            Name = name;
            Symbol = symbol;
        }

        /// <summary>
        /// The unique CoinMarketCap ID for this cryptocurrency
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; }

        /// <summary>
        /// The name of this cryptocurrency
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; }

        /// <summary>
        /// The ticker symbol for this cryptocurrency
        /// </summary>
        [JsonPropertyName("symbol")]
        public string Symbol { get; }
    }
}