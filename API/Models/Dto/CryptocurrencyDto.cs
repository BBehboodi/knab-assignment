namespace Knab.Assignment.API.Models.Dto
{
    public record class CryptocurrencyDto
    {
        public CryptocurrencyDto(int id, string name, string symbol)
        {
            Id = id;
            Name = name;
            Symbol = symbol;
        }

        public int Id { get; }

        public string Name { get; }

        public string Symbol { get; }

        public Cryptocurrency ToModel()
        {
            return new Cryptocurrency(Id, Name, Symbol);
        }
    }
}