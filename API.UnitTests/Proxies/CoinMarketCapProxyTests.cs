using Knab.Assignment.API.Configuration;
using Knab.Assignment.API.Exceptions;
using Knab.Assignment.API.Models.Proxy;
using Knab.Assignment.API.Proxies;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Knab.Assignment.API.UnitTests.Proxies
{
    public class CoinMarketCapProxyTests
    {
        private readonly Mock<IOptions<AppConfig>> _mockedAppConfig;
        private readonly Mock<ILogger<CoinMarketCapProxy>> _mockedLogger;

        public CoinMarketCapProxyTests()
        {
            _mockedAppConfig = new Mock<IOptions<AppConfig>>();
            _mockedLogger = new Mock<ILogger<CoinMarketCapProxy>>();
            _mockedLogger.Setup(x => x.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(),
                It.Is<CoinMarketCapProxy>((v, t) => true), It.IsAny<Exception>(),
                It.Is<Func<CoinMarketCapProxy, Exception?, string>>((v, t) => true))).Verifiable();

        }

        [Fact]
        public async Task GetCryptocurrencies_ValidSenario_ReturnsCryptocurrencies()
        {
            // Arrange
            int start = 1, limit = 100;
            string sort = "validSort", sortDir = "validSorDir";
            var status = new StatusResponse(0, errorMessage: null);
            var cryptocurrencies = new List<CryptocurrencyResponse>
            { new CryptocurrencyResponse(id: 1, name: "Bitcon", symbol: "BTC") };
            var response = new CryptocurrenciesResponse(status, cryptocurrencies);
            string responseJson = JsonSerializer.Serialize(response);
            var appConfig = new AppConfig { CoinMarketCap = new CoinMarketCapConfig { ApiKey = "Key", ApiUrl = "http://www.test.com" } };
            _mockedAppConfig.Setup(x => x.Value).Returns(appConfig);
            var mockedMessageHandler = GetMockedHttpClientHandler(HttpStatusCode.OK, new StringContent(responseJson));
            var httpClient = new HttpClient(mockedMessageHandler.Object)
            { BaseAddress = new Uri(_mockedAppConfig.Object.Value.CoinMarketCap.ApiUrl) };
            var coinMarketCapProxy = new CoinMarketCapProxy(_mockedLogger.Object, httpClient, _mockedAppConfig.Object);

            // Act
            var proxyResult = await coinMarketCapProxy.GetCryptocurrenciesAsync(start, limit, sort, sortDir);

            // Assert
            Assert.NotNull(proxyResult);
            Assert.Equal(cryptocurrencies.Count, proxyResult.Count);
            Assert.Equal(JsonSerializer.Serialize(cryptocurrencies), JsonSerializer.Serialize(proxyResult));
        }

        [Fact]
        public async Task GetCryptocurrencies_ResponseIsNot200_ThrowsHttpResponseException()
        {
            // Arrange
            int start = 1, limit = 100;
            string sort = "validSort", sortDir = "validSorDir";
            var status = new StatusResponse(400, errorMessage: "MyBadRequest");
            var response = new CryptocurrenciesResponse(status, cryptocurrencies: null);
            string responseJson = JsonSerializer.Serialize(response);
            var appConfig = new AppConfig { CoinMarketCap = new CoinMarketCapConfig { ApiKey = "Key", ApiUrl = "http://www.test.com" } };
            _mockedAppConfig.Setup(x => x.Value).Returns(appConfig);
            var mockedMessageHandler = GetMockedHttpClientHandler(HttpStatusCode.BadRequest, new StringContent(responseJson));
            var httpClient = new HttpClient(mockedMessageHandler.Object)
            { BaseAddress = new Uri(_mockedAppConfig.Object.Value.CoinMarketCap.ApiUrl) };
            var coinMarketCapProxy = new CoinMarketCapProxy(_mockedLogger.Object, httpClient, _mockedAppConfig.Object);

            // Assert
            await Assert.ThrowsAsync<HttpResponseException>(() => coinMarketCapProxy.GetCryptocurrenciesAsync(start, limit, sort, sortDir));
        }

        [Fact]
        public async Task GetCryptocurrencies_CryptocurrenciesIsNull_ThrowsInvalidOperationException()
        {
            // Arrange
            int start = 1, limit = 100;
            string sort = "validSort", sortDir = "validSorDir";
            var status = new StatusResponse(0, errorMessage: null);
            var response = new CryptocurrenciesResponse(status, cryptocurrencies: null);
            string responseJson = JsonSerializer.Serialize(response);
            var appConfig = new AppConfig { CoinMarketCap = new CoinMarketCapConfig { ApiKey = "Key", ApiUrl = "http://www.test.com" } };
            _mockedAppConfig.Setup(x => x.Value).Returns(appConfig);
            var mockedMessageHandler = GetMockedHttpClientHandler(HttpStatusCode.OK, new StringContent(responseJson));
            var httpClient = new HttpClient(mockedMessageHandler.Object)
            { BaseAddress = new Uri(_mockedAppConfig.Object.Value.CoinMarketCap.ApiUrl) };
            var coinMarketCapProxy = new CoinMarketCapProxy(_mockedLogger.Object, httpClient, _mockedAppConfig.Object);

            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => coinMarketCapProxy.GetCryptocurrenciesAsync(start, limit, sort, sortDir));
        }

        [Fact]
        public async Task GetQuotes_ValidSenario_ReturnsQuotes()
        {
            // Arrange
            string symbol = "BTC";
            var currencies = new string[] { "USD", "AUD" };
            var appConfig = new AppConfig { CoinMarketCap = new CoinMarketCapConfig { ApiKey = "Key", ApiUrl = "http://www.test.com" } };
            _mockedAppConfig.Setup(x => x.Value).Returns(appConfig);
            var quotes = currencies.ToDictionary(k => k, v => new QuoteInfoResponse(100));
            var cryptocurrenciesQuotes = new Dictionary<string, List<CryptocurrencyQuoteResponse>>
            { { symbol, new List<CryptocurrencyQuoteResponse> { new CryptocurrencyQuoteResponse(1, "Bitcoin", symbol, quotes) } } };
            var status = new StatusResponse(0, errorMessage: null);
            var response = new CryptocurrenciesQuoteResponse(status, cryptocurrenciesQuotes);
            string responseJson = JsonSerializer.Serialize(response);
            var mockedMessageHandler = GetMockedHttpClientHandler(HttpStatusCode.OK, new StringContent(responseJson));
            var httpClient = new HttpClient(mockedMessageHandler.Object)
            { BaseAddress = new Uri(_mockedAppConfig.Object.Value.CoinMarketCap.ApiUrl) };
            var coinMarketCapProxy = new CoinMarketCapProxy(_mockedLogger.Object, httpClient, _mockedAppConfig.Object);

            // Act
            var proxyResult = await coinMarketCapProxy.GetQuotesAsync(symbol, currencies);

            // Assert
            Assert.NotNull(proxyResult);
            Assert.Equal(quotes.Count, proxyResult.Count);
            Assert.All(currencies, currency => proxyResult.Any(x => x.Currency == currency));
        }

        [Fact]
        public async Task GetQuotes_ResponseIsNot200_ThrowsHttpResponseException()
        {
            // Arrange
            string symbol = "BTC";
            var currencies = new string[] { "USD", "AUD" };
            var status = new StatusResponse(400, errorMessage: "MyBadRequest");
            var response = new CryptocurrenciesQuoteResponse(status, cryptocurrenciesQuotes: null);
            string responseJson = JsonSerializer.Serialize(response);
            var mockedMessageHandler = GetMockedHttpClientHandler(HttpStatusCode.OK, new StringContent(responseJson));
            var appConfig = new AppConfig { CoinMarketCap = new CoinMarketCapConfig { ApiKey = "Key", ApiUrl = "http://www.test.com" } };
            _mockedAppConfig.Setup(x => x.Value).Returns(appConfig);
            var httpClient = new HttpClient(mockedMessageHandler.Object)
            { BaseAddress = new Uri(_mockedAppConfig.Object.Value.CoinMarketCap.ApiUrl) };
            var coinMarketCapProxy = new CoinMarketCapProxy(_mockedLogger.Object, httpClient, _mockedAppConfig.Object);

            // Assert
            await Assert.ThrowsAsync<HttpResponseException>(() => coinMarketCapProxy.GetQuotesAsync(symbol, currencies));
        }

        [Fact]
        public async Task GetQuotes_QuotesIsNull_ThrowsInvalidOperationException()
        {
            // Arrange
            string symbol = "BTC";
            var currencies = new string[] { "USD", "AUD" };
            var status = new StatusResponse(0, errorMessage: null);
            var response = new CryptocurrenciesQuoteResponse(status, cryptocurrenciesQuotes: null);
            string responseJson = JsonSerializer.Serialize(response);
            var mockedMessageHandler = GetMockedHttpClientHandler(HttpStatusCode.OK, new StringContent(responseJson));
            var appConfig = new AppConfig { CoinMarketCap = new CoinMarketCapConfig { ApiKey = "Key", ApiUrl = "http://www.test.com" } };
            _mockedAppConfig.Setup(x => x.Value).Returns(appConfig);
            var httpClient = new HttpClient(mockedMessageHandler.Object)
            { BaseAddress = new Uri(_mockedAppConfig.Object.Value.CoinMarketCap.ApiUrl) };
            var coinMarketCapProxy = new CoinMarketCapProxy(_mockedLogger.Object, httpClient, _mockedAppConfig.Object);

            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => coinMarketCapProxy.GetQuotesAsync(symbol, currencies));
        }

        private Mock<HttpMessageHandler> GetMockedHttpClientHandler(HttpStatusCode statusCode, HttpContent content)
        {
            var httpMessage = new HttpResponseMessage { StatusCode = statusCode, Content = content };
            var mockedMessageHandler = new Mock<HttpMessageHandler>();
            mockedMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).ReturnsAsync(httpMessage);
            return mockedMessageHandler;
        }
    }
}