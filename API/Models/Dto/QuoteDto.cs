namespace Knab.Assignment.API.Models.Dto
{
    public record class QuoteDto
    {
        public QuoteDto(string currency, decimal price)
        {
            Currency = currency;
            Price = price;
        }

        public string Currency { get; }

        public decimal Price { get; }

        public Quote ToModel()
        {
            return new Quote(Currency, Price);
        }
    }
}