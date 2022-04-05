using Knab.Assignment.API.ActionFilters;
using Knab.Assignment.API.Configuration;
using Knab.Assignment.API.Extensions;

namespace Knab.Assignment.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var appConfigSection = builder.Configuration.GetSection("AppConfig");
        var appConfig = appConfigSection.Get<AppConfig>();
        builder.Services.Configure<AppConfig>(appConfigSection);
        builder.Services.AddControllers(options =>
        {
            options.Filters.Add<HttpResponseExceptionFilter>();
        });
        builder.Services.AddSwagger();
        builder.Services.AddLogger();
        builder.Services.AddProxies(appConfig);
        builder.Services.AddServices();
        builder.Services.AddOptions();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage()
               .UseSwagger(setup => { setup.SerializeAsV2 = true; })
               .UseSwaggerUI(setup => setup.SwaggerEndpoint("/swagger/v1/swagger.json", "Cryptocurrency v1"));
        }
        app.UseHttpsRedirection()
           .UseRouting()
           .UseEndpoints(endpoints => endpoints.MapControllers());

        app.Run();
    }
}