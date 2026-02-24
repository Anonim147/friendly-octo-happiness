using AwesomeAssertions;
using NUnit.Framework;
using System.Net;
using TravelGuideApi.IntegrationTests.Helpers;

namespace TravelGuideApi.IntegrationTests.Tests;

public class HealthCheckTests
{
    private TestHost _testHost;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _testHost = new();
    }

    [OneTimeTearDown]
    public Task OneTimeTearDown() => _testHost.StopAndDispose();

    [Test]
    public async Task Given_PingRequest_Should_ReturnOk()
    {
        // Act
        using HttpResponseMessage response = await _testHost.HttpClient
            .GetAsync("/api/ping");

        string responseString = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        responseString.Should().Be("test 1.0.0");
    }
}
