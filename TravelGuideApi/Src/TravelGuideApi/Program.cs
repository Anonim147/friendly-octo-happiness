using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System;

namespace TravelGuideApi;

public class Program
{
    private const string StructuredOutputTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}";
    private const string PlainOutputTemplate = "{Message:lj}{NewLine}{Exception}";

    public static void Main(string[] args)
    {
        Log.Logger = CreateLoggerConfiguration(UseStructuredLogging())
            .CreateLogger();

        try
        {
            CreateHostBuilder(args).Build().Run();
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, "An unhandled error occurred. See exception details.");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            })
            .UseSerilog((context, services, configuration) =>
            {
                configuration
                    .MinimumLevel.Information()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                    .Enrich.FromLogContext()
                    .Enrich.WithEnvironmentName()
                    .Enrich.WithThreadId()
                    .WriteTo.Console(outputTemplate: UseStructuredLogging()
                        ? StructuredOutputTemplate
                        : PlainOutputTemplate);
            });
    }

    private static LoggerConfiguration CreateLoggerConfiguration(bool useStructuredLogging)
    {
        return new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .Enrich.WithEnvironmentName()
            .Enrich.WithThreadId()
            .WriteTo.Console(outputTemplate: useStructuredLogging
                ? StructuredOutputTemplate
                : PlainOutputTemplate);
    }

    private static bool UseStructuredLogging()
    {
        bool.TryParse(
            Environment.GetEnvironmentVariable(
                "USE_PLAIN_LOGGING",
                EnvironmentVariableTarget.Process),
            out bool usePlainLogging);

        return !usePlainLogging;
    }
}
