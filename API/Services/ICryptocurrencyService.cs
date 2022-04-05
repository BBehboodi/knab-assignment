using Knab.Assignment.API.Models.Dto;

namespace Knab.Assignment.API.Services
{
    public interface ICryptocurrencyService
    {
        Task<List<CryptocurrencyDto>> GetCryptocurrenciesAsync(int start, int limit, string sort, string sortDir);

        Task<List<QuoteDto>> GetQuotesAsync(string symbol);
    }
}