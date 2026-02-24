using Microsoft.AspNetCore.Builder;

namespace TravelGuideApi.Infrastructure.Extensions;

/// <summary>
/// Defines extension methods for <see cref="IApplicationBuilder"/>.
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Adds middlewares for Swagger and Swagger UI.
    /// </summary>
    /// <param name="app"><see cref="IApplicationBuilder"/></param>
    public static void UseSwaggerService(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "TravelGuideApi API V1");
            c.RoutePrefix = "swagger";
            c.DisplayRequestDuration();
        });
    }
}
