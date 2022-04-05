using Knab.Assignment.API.Configuration;
using Knab.Assignment.API.Models.Proxy;
using Knab.Assignment.API.Proxies;
using Knab.Assignment.API.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Knab.Assignment.API.UnitTests.Services
{
    public class CurrencyServiceTests
    {
        private readonly ICryptocurrencyService cryptocurrencyService;
        private readonly Mock<ICoinMarketCapProxy> _mockedCoinMarketCapProxy;
        private readonly Mock<IOptions<AppConfig>> _mockedAppConfig;

        public CurrencyServiceTests()
        {
            _mockedAppConfig = new Mock<IOptions<AppConfig>>();
            _mockedCoinMarketCapProxy = new Mock<ICoinMarketCapProxy>();

            var mockedLogger = new Mock<ILogger<CryptocurrencyService>>();
            mockedLogger.Setup(x => x.Log(
                It.IsAny<LogLevel>(), It.IsAny<EventId>(),
                It.Is<CryptocurrencyService>((v, t) => true), It.IsAny<Exception>(),
                It.Is<Func<CryptocurrencyService, Exception?, string>>((v, t) => true))).Verifiable();

            cryptocurrencyService = new CryptocurrencyService(
                mockedLogger.Object,
                _mockedCoinMarketCapProxy.Object,
                _mockedAppConfig.Object);
        }

        [Fact]
        public async Task GetCryptocurrencies_ValidSenario_ReturnsCryptocurrencies()
        {
            // Arrange
            int start = 1, limit = 100;
            string sort = "name", sortDir = "asc";
            var cryptocurrencies = new List<CryptocurrencyResponse>
            { new CryptocurrencyResponse(id: 1, name: "Bitcoin", symbol: "BTC") };
            _mockedCoinMarketCapProxy.Setup(x => x.GetCryptocurrenciesAsync(start, limit, sort, sortDir)).ReturnsAsync(cryptocurrencies);

            // Act
            var serviceResult = await cryptocurrencyService.GetCryptocurrenciesAsync(start, limit, sort, sortDir);

            // Assert
            Assert.NotNull(serviceResult);
            Assert.Single(serviceResult);
            _mockedCoinMarketCapProxy.Verify(x => x.GetCryptocurrenciesAsync(start, limit, sort, sortDir), Times.Once);
        }

        [Fact]
        public async Task GetCryptocurrencies_ResponseIsNull_ThrowsInvalidOperationException()
        {
            // Arrange
            int start = 1, limit = 100;
            string sort = "name", sortDir = "asc";
            _mockedCoinMarketCapProxy.Setup(x => x.GetCryptocurrenciesAsync(start, limit, sort, sortDir)).ReturnsAsync(() => null!);

            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => cryptocurrencyService.GetCryptocurrenciesAsync(start, limit, sort, sortDir));
            _mockedCoinMarketCapProxy.Verify(x => x.GetCryptocurrenciesAsync(start, limit, sort, sortDir), Times.Once);
        }

        [Fact]
        public async Task GetCryptocurrencies_StartLessThanOne_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            int start = -1, limit = 100;
            string sort = "name", sortDir = "asc";

            // Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => cryptocurrencyService.GetCryptocurrenciesAsync(start, limit, sort, sortDir));
            _mockedCoinMarketCapProxy.Verify(x => x.GetCryptocurrenciesAsync(start, limit, sort, sortDir), Times.Never);
        }

        [Fact]
        public async Task GetCryptocurrencies_LimitLessThanOne_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            int start = 1, limit = -1;
            string sort = "name", sortDir = "asc";

            // Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => cryptocurrencyService.GetCryptocurrenciesAsync(start, limit, sort, sortDir));
            _mockedCoinMarketCapProxy.Verify(x => x.GetCryptocurrenciesAsync(start, limit, sort, sortDir), Times.Never);
        }

        [Fact]
        public async Task GetCryptocurrencies_SortIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            int start = 1, limit = 100;
            string sort = null!, sortDir = "asc";

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => cryptocurrencyService.GetCryptocurrenciesAsync(start, limit, sort, sortDir));
            _mockedCoinMarketCapProxy.Verify(x => x.GetCryptocurrenciesAsync(start, limit, sort, sortDir), Times.Never);
        }

        [Fact]
        public async Task GetCryptocurrencies_SortIsEmpty_ThrowsArgumentException()
        {
            // Arrange
            int start = 1, limit = 100;
            string sort = string.Empty, sortDir = "asc";

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(() => cryptocurrencyService.GetCryptocurrenciesAsync(start, limit, sort, sortDir));
            _mockedCoinMarketCapProxy.Verify(x => x.GetCryptocurrenciesAsync(start, limit, sort, sortDir), Times.Never);
        }

        [Fact]
        public async Task GetCryptocurrencies_SortDirIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            int start = 1, limit = 100;
            string sort = "name", sortDir = null!;

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => cryptocurrencyService.GetCryptocurrenciesAsync(start, limit, sort, sortDir));
            _mockedCoinMarketCapProxy.Verify(x => x.GetCryptocurrenciesAsync(start, limit, sort, sortDir), Times.Never);
        }

        [Fact]
        public async Task GetCryptocurrencies_SortDirIsEmpty_ThrowsArgumentException()
        {
            // Arrange
            int start = 1, limit = 100;
            string sort = "name", sortDir = string.Empty;

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(() => cryptocurrencyService.GetCryptocurrenciesAsync(start, limit, sort, sortDir));
            _mockedCoinMarketCapProxy.Verify(x => x.GetCryptocurrenciesAsync(start, limit, sort, sortDir), Times.Never);
        }

        [Fact]
        public async Task GetQuotes_ValidSenario_ReturnsQuotes()
        {
            // Arrange
            string symbol = "BTC";
            var appConfig = new AppConfig { Currencies = new string[] { "AUR", "USD" } };
            var quotes = appConfig.Currencies.Select(x => new QuoteResponse(x, 100)).ToList();
            _mockedAppConfig.Setup(x => x.Value).Returns(appConfig);
            _mockedCoinMarketCapProxy.Setup(x => x.GetQuotesAsync(symbol, appConfig.Currencies)).ReturnsAsync(quotes);

            // Act
            var serviceResult = await cryptocurrencyService.GetQuotesAsync(symbol);

            // Assert
            Assert.NotNull(serviceResult);
            Assert.True(serviceResult.Count == appConfig.Currencies.Length);
            _mockedCoinMarketCapProxy.Verify(x => x.GetQuotesAsync(symbol, appConfig.Currencies), Times.Once);
        }

        [Fact]
        public async Task GetQuotes_ResponseIsNull_ThrowsInvalidOperationException()
        {
            // Arrange
            string symbol = "BTC";
            var appConfig = new AppConfig { Currencies = new string[] { "AUR", "USD" } };
            _mockedAppConfig.Setup(x => x.Value).Returns(appConfig);
            _mockedCoinMarketCapProxy.Setup(x => x.GetQuotesAsync(symbol, appConfig.Currencies)).ReturnsAsync(() => null!);

            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => cryptocurrencyService.GetQuotesAsync(symbol));
            _mockedCoinMarketCapProxy.Verify(x => x.GetQuotesAsync(symbol, appConfig.Currencies), Times.Once);
        }

        [Fact]
        public async Task GetQuotes_SymbolIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            string symbol = null!;
            var appConfig = new AppConfig { Currencies = new string[] { "AUR", "USD" } };
            var quotes = appConfig.Currencies.Select(x => new QuoteResponse(x, 100)).ToList();

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => cryptocurrencyService.GetQuotesAsync(symbol));
            _mockedCoinMarketCapProxy.Verify(x => x.GetQuotesAsync(symbol, appConfig.Currencies), Times.Never);
        }

        [Fact]
        public async Task GetQuotes_SymbolIsEmpty_ThrowsArgumentException()
        {
            // Arrange
            string symbol = "";
            var appConfig = new AppConfig { Currencies = new string[] { "AUR", "USD" } };
            var quotes = appConfig.Currencies.Select(x => new QuoteResponse(x, 100)).ToList();

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(() => cryptocurrencyService.GetQuotesAsync(symbol));
            _mockedCoinMarketCapProxy.Verify(x => x.GetQuotesAsync(symbol, appConfig.Currencies), Times.Never);
        }
    }
}