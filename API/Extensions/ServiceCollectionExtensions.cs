using Knab.Assignment.API.Configuration;
using Knab.Assignment.API.Proxies;
using Knab.Assignment.API.Services;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace Knab.Assignment.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(setup =>
            {
                var contact = new OpenApiContact
                {
                    Name = "Behzad Behboodi",
                    Email = "behboodi.b71@gmail.com",
                    Url = new Uri("https://www.linkedin.com/in/behzadbehboodi")
                };
                var apiInfo = new OpenApiInfo
                {
                    Title = "Cryptocurrency",
                    Version = "v1",
                    Contact = contact,
                    Description = "Knab Coding Assessment"
                };
                setup.SwaggerDoc(name: "v1", apiInfo);
                string xmlDocument = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                string xmlDocumentPath = Path.Combine(AppContext.BaseDirectory, xmlDocument);
                setup.IncludeXmlComments(xmlDocumentPath);
            });
            return services;
        }

        public static IServiceCollection AddLogger(this IServiceCollection services)
        {
            services.AddLogging(logging => logging.AddLog4Net("log4net.config"));
            return services;
        }

        public static IServiceCollection AddProxies(this IServiceCollection services, AppConfig appConfig)
        {
            services.AddHttpClient<ICoinMarketCapProxy, CoinMarketCapProxy>(x =>
            {
                x.BaseAddress = new Uri(appConfig.CoinMarketCap.ApiUrl);
                x.DefaultRequestHeaders.Add("X-CMC_PRO_API_KEY", appConfig.CoinMarketCap.ApiKey);
            });
            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<ICryptocurrencyService, CryptocurrencyService>();
            return services;
        }
    }
}