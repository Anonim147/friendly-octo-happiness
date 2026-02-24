using AwesomeAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TravelGuideApi.Application.Features.Country.Queries.GetAll;
using TravelGuideApi.Application.Features.CurrencyComparison.Queries.Compare;
using TravelGuideApi.Controllers.v1;
using TravelGuideApi.Domain.Entities;
using TravelGuideApi.Domain.Models;

namespace TravelGuideApi.UnitTests.Controllers.v1;

[TestFixture]
public class ComparisonControllerTests
{
    private Mock<ILogger<ComparisonController>> _loggerMock = null!;
    private Mock<IMediator> _mediatorMock = null!;
    private ComparisonController _controller = null!;

    [SetUp]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger<ComparisonController>>();
        _mediatorMock = new Mock<IMediator>();
        _controller = new ComparisonController(_loggerMock.Object, _mediatorMock.Object);
    }

    #region GetCountries Tests

    [Test]
    public async Task GetCountries_Should_ReturnOkWithCountries()
    {
        // Arrange
        var expectedCountries = new List<CountryEntity>
        {
            new() { Code = "US", Name = "United States", CurrencyCode = "USD" },
            new() { Code = "GB", Name = "United Kingdom", CurrencyCode = "GBP" }
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CountryGetAllQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedCountries);

        // Act
        var result = await _controller.GetCountries();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(expectedCountries);
    }

    [Test]
    public async Task GetCountries_Should_ReturnEmptyList_WhenNoCountries()
    {
        // Arrange
        var emptyList = new List<CountryEntity>();

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CountryGetAllQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyList);

        // Act
        var result = await _controller.GetCountries();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(emptyList);
    }

    [Test]
    public async Task GetCountries_Should_CallMediator()
    {
        // Arrange
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CountryGetAllQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CountryEntity>());

        // Act
        await _controller.GetCountries();

        // Assert
        _mediatorMock.Verify(m => m.Send(It.IsAny<CountryGetAllQuery>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region CompareCurrency Tests

    [Test]
    public async Task CompareCurrency_Should_ReturnOkWithResult()
    {
        // Arrange
        var request = new CurrencyCompareQuery("US", "GB");
        var expectedResult = new CurrencyComparisonResult
        {
            HomeCountry = "United States",
            HomeCurrency = "USD",
            DestinationCountry = "United Kingdom",
            DestinationCurrency = "GBP",
            ExchangeRate = 0.79m,
            StrengthHint = "Your money is weaker"
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CurrencyCompareQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.CompareCurrency(request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(expectedResult);
    }

    [Test]
    public async Task CompareCurrency_Should_CallMediatorWithCorrectRequest()
    {
        // Arrange
        var request = new CurrencyCompareQuery("US", "GB");
        var result = new CurrencyComparisonResult();

        _mediatorMock
            .Setup(m => m.Send(It.Is<CurrencyCompareQuery>(q => 
                q.HomeCountryCode == "US" && q.DestinationCountryCode == "GB"), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        await _controller.CompareCurrency(request);

        // Assert
        _mediatorMock.Verify(m => m.Send(It.Is<CurrencyCompareQuery>(q => 
            q.HomeCountryCode == "US" && q.DestinationCountryCode == "GB"), 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task CompareCurrency_Should_ReturnCorrectStrengthHint_WhenMoneyIsStronger()
    {
        // Arrange
        var request = new CurrencyCompareQuery("US", "IN");
        var expectedResult = new CurrencyComparisonResult
        {
            HomeCountry = "United States",
            HomeCurrency = "USD",
            DestinationCountry = "India",
            DestinationCurrency = "INR",
            ExchangeRate = 83.12m,
            StrengthHint = "Your money is stronger"
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CurrencyCompareQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.CompareCurrency(request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        var comparisonResult = okResult!.Value as CurrencyComparisonResult;
        comparisonResult!.StrengthHint.Should().Be("Your money is stronger");
    }

    #endregion
}
