using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace TravelGuideApi.IntegrationTests.Helpers;

public class TestHost
{
    private readonly TestServer _testServer;

    public TestHost(Action<IServiceCollection>? configureMock = null)
    {
        WebApplicationFactory<Startup> factory = new WebApplicationFactory<Startup>()
           .WithWebHostBuilder(builder => builder
               .ConfigureAppConfiguration((context, config) => config.AddJsonFile("appsettings.json"))
               .ConfigureServices(services =>
               {
                   services
                       .AddSingleton<ILoggerFactory, NullLoggerFactory>();
                   configureMock?.Invoke(services);
               }));

        _testServer = factory.Server;
        HttpClient = _testServer.CreateClient();
    }

    public HttpClient HttpClient { get; }

    public TService GetService<TService>() where TService : class => _testServer.Services.GetRequiredService<TService>();

    public async Task StopAndDispose()
    {
        try
        {
            await _testServer.Host.StopAsync();
        }
        catch
        {
        }

        _testServer.Dispose();
    }
}
