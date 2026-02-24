using Asp.Versioning;
using FluentValidation;
using Hellang.Middleware.ProblemDetails;
using TravelGuideApi.Application.Extensions;
using TravelGuideApi.Infrastructure.Extensions;
using TravelGuideApi.Persistence.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;



using System.Linq;

namespace TravelGuideApi;

public class Startup(
    IWebHostEnvironment hostEnvironment)
{
    private readonly IWebHostEnvironment _hostEnvironment = hostEnvironment;

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        IConfiguration configuration = services.AddConfiguration();

        services.AddApiVersioning(config =>
        {
            // Specify the default API Version as 1.0
            config.DefaultApiVersion = new ApiVersion(1, 0);
            // If the client hasn't specified the API version in the request, use the default API version number
            config.AssumeDefaultVersionWhenUnspecified = true;
            // Advertise the API versions supported for the particular endpoint
            config.ReportApiVersions = true;
        });

        services.AddHttpContextAccessor();
        services.AddProblemDetails(opts =>
        {
            opts.Map<ValidationException>(ex =>
            {
                return new ValidationProblemDetails(ex.Errors
                    .GroupBy(f => f.PropertyName, f => f.ErrorMessage)
                    .ToDictionary(f => f.Key, f => f.Select(x => x).ToArray()))
                {
                    Status = 400
                };
            });

            opts.Map<System.ArgumentException>(ex =>
            {
                return new ProblemDetails
                {
                    Status = 400,
                    Title = "Bad Request",
                    Detail = ex.Message
                };
            });
        });

        services.AddSharedInfrastructure();
        services.AddApplication();
        services.AddPersistence();

        services.AddControllers();

        services.AddSwaggerService();
        services.AddOpenApiDocument();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app)
    {
        app.UseProblemDetails();

        app.UseSwaggerService();



        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
