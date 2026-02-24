using AwesomeAssertions;
using TravelGuideApi.Controllers;
using TravelGuideApi.Domain.Models;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace TravelGuideApi.UnitTests.Controllers;

[TestFixture]
public class PingControllerTests
{
    private Mock<IOptions<Config>> _optionsMock = null!;

    [SetUp]
    public void Setup()
    {
        _optionsMock = new(MockBehavior.Strict);
    }

    [Test]
    public void Get_Should_ReturnPingValueFromConfig()
    {
        // Arrange
        _optionsMock.SetupGet(x => x.Value).Returns(new Config("ping", "https://restcountries.com/v3.1", "https://api.exchangerate.host"));
        var controller = new PingController(_optionsMock.Object);

        // Act
        string result = controller.Ping();

        // Assert
        result.Should().Be("ping");
    }
}
