using FluentValidation;
using TravelGuideApi.Application.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace TravelGuideApi.Application.Extensions;

/// <summary>
/// Defines extensions methods for <see cref="IServiceCollection"/>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers dependencies of Application layer.
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddSingleton(TimeProvider.System);
    }
}
