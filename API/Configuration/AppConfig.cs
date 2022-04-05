#nullable disable

namespace Knab.Assignment.API.Configuration
{
    public class AppConfig
    {
        public CoinMarketCapConfig CoinMarketCap { get; set; }

        public string[] Currencies { get; set; }
    }
}