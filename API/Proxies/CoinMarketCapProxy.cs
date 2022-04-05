using Knab.Assignment.API.Configuration;
using Knab.Assignment.API.Exceptions;
using Knab.Assignment.API.Models.Proxy;
using Microsoft.Extensions.Options;

namespace Knab.Assignment.API.Proxies
{
    public class CoinMarketCapProxy : ICoinMarketCapProxy
    {
        private readonly ILogger _logger;
        private readonly IOptions<AppConfig> _options;
        private readonly HttpClient _httpClient;

        public CoinMarketCapProxy(ILogger<CoinMarketCapProxy> logger, HttpClient httpClient, IOptions<AppConfig> options)
        {
            _logger = logger;
            _options = options;
            _httpClient = httpClient;
        }

        public async Task<List<CryptocurrencyResponse>> GetCryptocurrenciesAsync(int start, int limit, string sort, string sortDir)
        {
            _logger.LogTrace($"{nameof(GetCryptocurrenciesAsync)} proxy called. " +
                $"args: {nameof(start)}: {start}, {nameof(limit)}: {limit}, {nameof(sort)}: {sort}, {nameof(sortDir)}: {sortDir}.");

            var requestUrl = $"v1/cryptocurrency/listings/latest?start={start}&limit={limit}&sort={sort}&sort_dir={sortDir}";
            var httpMessage = await _httpClient.GetAsync(requestUrl);
            var response = await httpMessage.Content.ReadFromJsonAsync<CryptocurrenciesResponse>();

            if (response is null)
                throw new InvalidOperationException($"{nameof(response)} could not be null");

            if (!response.Succeed)
                throw new HttpResponseException(response.Status.ErrorCode, response.Status.ErrorMessage!);

            if (response.Cryptocurrencies is null)
                throw new InvalidOperationException($"{nameof(response.Cryptocurrencies)} could not be null");

            return response.Cryptocurrencies;
        }

        public async Task<List<QuoteResponse>> GetQuotesAsync(string symbol, string[] currencies)
        {
            _logger.LogTrace($"{nameof(GetQuotesAsync)} proxy called. " +
                $"args: {nameof(symbol)}: {symbol}, {nameof(currencies)}: {string.Join(',', currencies)}");

            if (_options.Value.CoinMarketCap.IsBasicPlan)
                return await GetQuotesBasicPlan(symbol, currencies);

            return await GetLatestQuotesAsync(symbol, currencies);
        }

        private async Task<List<QuoteResponse>> GetQuotesBasicPlan(string symbol, string[] currencies)
        {
            var responsesTasks = currencies
                .Select(currency => GetLatestQuotesAsync(symbol, new string[] { currency }))
                .ToList();
            var responses = await Task.WhenAll(responsesTasks);
            return responses.SelectMany(x => x).ToList();
        }

        public async Task<List<QuoteResponse>> GetLatestQuotesAsync(string symbol, string[] currencies)
        {
            var requestUrl = $"v2/cryptocurrency/quotes/latest?symbol={symbol}&convert={string.Join(',', currencies)}";
            var httpMessage = await _httpClient.GetAsync(requestUrl);
            var response = await httpMessage.Content.ReadFromJsonAsync<CryptocurrenciesQuoteResponse>();

            if (response is null)
                throw new InvalidOperationException($"{nameof(response)} could not be null");

            if (!response.Succeed)
                throw new HttpResponseException(response.Status.ErrorCode, response.Status.ErrorMessage!);

            return response.ToQuoteResponse();
        }
    }
}