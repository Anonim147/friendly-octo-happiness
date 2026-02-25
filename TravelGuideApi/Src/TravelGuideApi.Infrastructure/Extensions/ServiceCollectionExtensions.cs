using TravelGuideApi.Application.Interfaces;
using TravelGuideApi.Domain.Interfaces;
using TravelGuideApi.Domain.Models;
using TravelGuideApi.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace TravelGuideApi.Infrastructure.Extensions;

/// <summary>
/// Contains extensions methods for <see cref="IServiceCollection"/>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Configures Swagger and adds it to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/>.</param>
    public static void AddSwaggerService(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "TravelGuideApi API",
                Description = "TravelGuideApi API description"
            });
        });
    }

    /// <summary>
    /// Adds <see cref="Config"/> to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services"><see cref="IConfiguration"/></param>
    /// <returns><see cref="IConfiguration"/>.</returns>
    public static IConfiguration AddConfiguration(this IServiceCollection services)
    {
        IConfigurationBuilder configBuilder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json");
        IConfigurationRoot configuration = configBuilder.Build();
        services.Configure<Config>(configuration);
        return configuration;
    }

    /// <summary>
    /// Adds shared infrastructure services
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/>.</param>
    public static void AddSharedInfrastructure(this IServiceCollection services)
    {
        // Add memory cache
        services.AddMemoryCache();

        // Register cache services
        services.AddSingleton<ICountryCache, CountryCache>();
        services.AddSingleton<IExchangeRateCache, ExchangeRateCache>();

        // Register HttpClient for external API services
        services.AddHttpClient<ICountryApiService, CountryApiService>();
        services.AddHttpClient<IExchangeRateApiService, ExchangeRateApiService>();
        services.AddHttpClient<IExchangeRateHistoryService, ExchangeRateHistoryService>();
    }
}
