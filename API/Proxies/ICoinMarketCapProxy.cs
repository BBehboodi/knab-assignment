using Knab.Assignment.API.Models.Proxy;

namespace Knab.Assignment.API.Proxies
{
    public interface ICoinMarketCapProxy
    {
        Task<List<CryptocurrencyResponse>> GetCryptocurrenciesAsync(int start, int limit, string sort, string sortDir);

        Task<List<QuoteResponse>> GetQuotesAsync(string symbol, string[] currencies);
    }
}