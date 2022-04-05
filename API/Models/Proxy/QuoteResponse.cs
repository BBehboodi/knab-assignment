using Knab.Assignment.API.Models.Dto;

namespace Knab.Assignment.API.Models.Proxy
{
    public class QuoteResponse
    {
        public QuoteResponse(string currency, decimal price)
        {
            Currency = currency;
            Price = price;
        }

        public string Currency { get; }

        public decimal Price { get; }

        public QuoteDto ToDto()
        {
            return new QuoteDto(Currency, Price);
        }
    }
}