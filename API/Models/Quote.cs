using System.Text.Json.Serialization;

namespace Knab.Assignment.API.Models
{
    public class Quote
    {
        public Quote(string currency, decimal price)
        {
            Currency = currency;
            Price = price;
        }

        /// <summary>
        /// The name of the currency
        /// </summary>
        [JsonPropertyName("currency")]
        public string Currency { get; }

        /// <summary>
        /// The quote of each unit of currency
        /// </summary>
        [JsonPropertyName("price")]
        public decimal Price { get; }
    }
}