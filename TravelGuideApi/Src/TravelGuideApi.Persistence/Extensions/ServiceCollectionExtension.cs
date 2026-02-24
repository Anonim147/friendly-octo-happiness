using Microsoft.Extensions.DependencyInjection;

namespace TravelGuideApi.Persistence.Extensions;

/// <summary>
/// Contains extension methods for <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtension
{
    /// <summary>
    /// Registers dependencies of Persistence layer.
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    public static void AddPersistence(this IServiceCollection services)
    {
    }
}
