using Knab.Assignment.API.Configuration;
using Knab.Assignment.API.Models.Dto;
using Knab.Assignment.API.Proxies;
using Microsoft.Extensions.Options;

namespace Knab.Assignment.API.Services
{
    public class CryptocurrencyService : ICryptocurrencyService
    {
        private readonly ILogger _logger;
        private readonly ICoinMarketCapProxy _coinMarketCapProxy;
        private readonly IOptions<AppConfig> _options;

        public CryptocurrencyService(ILogger<CryptocurrencyService> logger, ICoinMarketCapProxy coinMarketCapProxy, IOptions<AppConfig> options)
        {
            _logger = logger;
            _coinMarketCapProxy = coinMarketCapProxy;
            _options = options;
        }

        public async Task<List<CryptocurrencyDto>> GetCryptocurrenciesAsync(int start, int limit, string sort, string sortDir)
        {
            _logger.LogTrace($"{nameof(GetCryptocurrenciesAsync)} service called. " +
                $"args: {nameof(start)}: {start}, {nameof(limit)}: {limit}, {nameof(sort)}: {sort}, {nameof(sortDir)}: {sortDir}.");

            if (sort is null)
                throw new ArgumentNullException(nameof(sort));

            if (sort.Trim() == string.Empty)
                throw new ArgumentException(nameof(sort));

            if (sortDir is null)
                throw new ArgumentNullException(nameof(sortDir));

            if (sortDir.Trim() == string.Empty)
                throw new ArgumentException(nameof(sortDir));

            if (start < 1)
                throw new ArgumentOutOfRangeException(nameof(start));

            if (limit < 1)
                throw new ArgumentOutOfRangeException(nameof(limit));

            var response = await _coinMarketCapProxy
                .GetCryptocurrenciesAsync(start, limit, sort, sortDir);

            if (response is null)
                throw new InvalidOperationException($"{nameof(response)} could not be null");

            return response
                .Select(x => x.ToDto())
                .ToList();
        }

        public async Task<List<QuoteDto>> GetQuotesAsync(string symbol)
        {
            _logger.LogTrace($"{nameof(GetQuotesAsync)} service called. args: {nameof(symbol)}: {symbol}");

            if (symbol is null)
                throw new ArgumentNullException(nameof(symbol));

            if (symbol.Trim() == string.Empty)
                throw new ArgumentException(nameof(symbol));

            var response = await _coinMarketCapProxy
                .GetQuotesAsync(symbol, _options.Value.Currencies);

            if (response is null)
                throw new InvalidOperationException($"{nameof(response)} could not be null");

            return response
                .Select(x => x.ToDto())
                .ToList();
        }
    }
}