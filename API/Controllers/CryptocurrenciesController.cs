using Knab.Assignment.API.Models;
using Knab.Assignment.API.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace Knab.Assignment.API.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/v1/cryptocurrencies")]
    public class CryptocurrenciesController : ControllerBase
    {
        private readonly ICryptocurrencyService _cryptocurrencyService;
        private readonly ILogger<CryptocurrenciesController> _logger;

        public CryptocurrenciesController(ICryptocurrencyService cryptocurrencyService, ILogger<CryptocurrenciesController> logger)
        {
            _cryptocurrencyService = cryptocurrencyService;
            _logger = logger;
        }

        /// <summary>
        /// Paginated list of cryptocurrencies
        /// </summary>
        /// <param name="start">Optionally offset the start (1-based index) of the paginated list of items to return.</param>
        /// <param name="limit">Optionally specify the number of results to return. Use this parameter and the "start" parameter to determine your own pagination size.</param>
        /// <param name="sort">What field to sort the list of cryptocurrencies by.
        ///  Valid values: "name", "symbol", "date_added", "market_cap", "market_cap_strict", "price", "circulating_supply", "total_supply", "max_supply", "num_market_pairs", "volume_24h", "percent_change_1h", "percent_change_24h", "percent_change_7d", "market_cap_by_total_supply_strict", "volume_7d", "volume_30d"</param>
        /// <param name="sortDir">The direction in which to order cryptocurrencies against the specified sort.
        /// Valid values: "asc", "desc"</param>
        /// <returns>Returns a paginated list of all active cryptocurrencies with latest market data.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(Cryptocurrency[]), StatusCodes.Status200OK)]
        public async Task<List<Cryptocurrency>> GetCryptocurrenciesAsync([DefaultValue(1)] int start, [DefaultValue(100)] int limit,
            [DefaultValue("market_cap")] string sort, [DefaultValue("asc")] string sortDir)
        {
            _logger.LogTrace($"{nameof(GetCryptocurrenciesAsync)} api called. " +
                $"args: {nameof(start)}: {start}, {nameof(limit)}: {limit}, {nameof(sort)}: {sort}, {nameof(sortDir)}: {sortDir}.");

            var cryptocurrencies = await _cryptocurrencyService
                .GetCryptocurrenciesAsync(start, limit, sort, sortDir);

            var response = cryptocurrencies
                .Select(x => x.ToModel())
                .ToList();

            return response;
        }

        /// <summary>
        /// Quote of cryptocurrencies
        /// </summary>
        /// <param name="symbol">The ticker symbol for this cryptocurrency.</param>
        /// <returns>Returns the latest market quote for 1 cryptocurrency.</returns>
        [HttpGet("{symbol}/quotes")]
        [ProducesResponseType(typeof(CryptocurrencyQuote[]), StatusCodes.Status200OK)]
        public async Task<List<CryptocurrencyQuote>> GetQuotesAsync([DefaultValue("BTC")] string symbol)
        {
            _logger.LogTrace($"{nameof(GetQuotesAsync)} api called. args: {nameof(symbol)}: {symbol}");

            var quotes = await _cryptocurrencyService
                .GetQuotesAsync(symbol);

            var response = quotes
                .Select(x=> x.ToModel())
                .ToList();

            return response;
        }
    }
}