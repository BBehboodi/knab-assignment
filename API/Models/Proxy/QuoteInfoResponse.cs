using System.Text.Json.Serialization;

namespace Knab.Assignment.API.Models.Proxy
{
    public class QuoteInfoResponse
    {
        [JsonConstructor]
        public QuoteInfoResponse(decimal price)
        {
            Price = price;
        }

        [JsonPropertyName("price")]
        public decimal Price { get; }
    }
}