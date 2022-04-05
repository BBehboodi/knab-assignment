#nullable disable

namespace Knab.Assignment.API.Configuration
{
    public class CoinMarketCapConfig
    {
        public string ApiUrl { get; set; }

        public string ApiKey { get; set; }

        public bool IsBasicPlan { get; set; }
    }
}