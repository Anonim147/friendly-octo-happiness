using AwesomeAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using TravelGuideApi.Application.Features.CurrencyComparison.Queries.Compare;
using TravelGuideApi.Application.Interfaces;
using TravelGuideApi.Domain.Entities;
using TravelGuideApi.Domain.Interfaces;

namespace TravelGuideApi.Application.UnitTests.Features.CurrencyComparison;

[TestFixture]
public class CurrencyCompareQueryHandlerTests
{
    private Mock<ICountryApiService> _countryApiServiceMock = null!;
    private Mock<IExchangeRateApiService> _exchangeRateApiServiceMock = null!;
    private Mock<ICountryCache> _countryCacheMock = null!;
    private Mock<ILogger<CurrencyCompareQueryHandler>> _loggerMock = null!;
    private CurrencyCompareQueryHandler _handler = null!;

    [SetUp]
    public void Setup()
    {
        _countryApiServiceMock = new Mock<ICountryApiService>();
        _exchangeRateApiServiceMock = new Mock<IExchangeRateApiService>();
        _countryCacheMock = new Mock<ICountryCache>();
        _loggerMock = new Mock<ILogger<CurrencyCompareQueryHandler>>();

        _handler = new CurrencyCompareQueryHandler(
            _countryApiServiceMock.Object,
            _exchangeRateApiServiceMock.Object,
            _countryCacheMock.Object,
            _loggerMock.Object);
    }

    #region Country Resolution Tests

    [Test]
    public async Task Handle_Should_GetCountriesFromCache_WhenAvailable()
    {
        // Arrange
        var homeCountry = CreateCountry("US", "United States", "USD");
        var destCountry = CreateCountry("GB", "United Kingdom", "GBP");

        _countryCacheMock.Setup(c => c.GetByCodeAsync("US")).ReturnsAsync(homeCountry);
        _countryCacheMock.Setup(c => c.GetByCodeAsync("GB")).ReturnsAsync(destCountry);
        _exchangeRateApiServiceMock.Setup(e => e.GetExchangeRateAsync("USD", "GBP")).ReturnsAsync(0.79m);

        var query = new CurrencyCompareQuery("US", "GB");

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _countryApiServiceMock.Verify(s => s.GetCountryByCodeAsync(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task Handle_Should_GetCountriesFromApi_WhenCacheMiss()
    {
        // Arrange
        var homeCountry = CreateCountry("US", "United States", "USD");
        var destCountry = CreateCountry("GB", "United Kingdom", "GBP");

        _countryCacheMock.Setup(c => c.GetByCodeAsync("US")).ReturnsAsync((CountryEntity?)null);
        _countryCacheMock.Setup(c => c.GetByCodeAsync("GB")).ReturnsAsync((CountryEntity?)null);
        _countryApiServiceMock.Setup(s => s.GetCountryByCodeAsync("US")).ReturnsAsync(homeCountry);
        _countryApiServiceMock.Setup(s => s.GetCountryByCodeAsync("GB")).ReturnsAsync(destCountry);
        _exchangeRateApiServiceMock.Setup(e => e.GetExchangeRateAsync("USD", "GBP")).ReturnsAsync(0.79m);

        var query = new CurrencyCompareQuery("US", "GB");

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _countryApiServiceMock.Verify(s => s.GetCountryByCodeAsync("US"), Times.Once);
        _countryApiServiceMock.Verify(s => s.GetCountryByCodeAsync("GB"), Times.Once);
    }

    [Test]
    public async Task Handle_Should_ThrowArgumentException_WhenHomeCountryNotFound()
    {
        // Arrange
        _countryCacheMock.Setup(c => c.GetByCodeAsync("XX")).ReturnsAsync((CountryEntity?)null);
        _countryApiServiceMock.Setup(s => s.GetCountryByCodeAsync("XX")).ReturnsAsync((CountryEntity?)null);
        _countryCacheMock.Setup(c => c.GetByCodeAsync("GB")).ReturnsAsync(CreateCountry("GB", "United Kingdom", "GBP"));

        var query = new CurrencyCompareQuery("XX", "GB");

        // Act
        Func<Task> act = () => _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*XX*");
    }

    [Test]
    public async Task Handle_Should_ThrowArgumentException_WhenDestinationCountryNotFound()
    {
        // Arrange
        var homeCountry = CreateCountry("US", "United States", "USD");
        _countryCacheMock.Setup(c => c.GetByCodeAsync("US")).ReturnsAsync(homeCountry);
        _countryCacheMock.Setup(c => c.GetByCodeAsync("YY")).ReturnsAsync((CountryEntity?)null);
        _countryApiServiceMock.Setup(s => s.GetCountryByCodeAsync("YY")).ReturnsAsync((CountryEntity?)null);

        var query = new CurrencyCompareQuery("US", "YY");

        // Act
        Func<Task> act = () => _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*YY*");
    }

    #endregion

    #region Exchange Rate Tests

    [Test]
    public async Task Handle_Should_ReturnCorrectExchangeRate()
    {
        // Arrange
        var homeCountry = CreateCountry("US", "United States", "USD");
        var destCountry = CreateCountry("IN", "India", "INR");

        _countryCacheMock.Setup(c => c.GetByCodeAsync("US")).ReturnsAsync(homeCountry);
        _countryCacheMock.Setup(c => c.GetByCodeAsync("IN")).ReturnsAsync(destCountry);
        _exchangeRateApiServiceMock.Setup(e => e.GetExchangeRateAsync("USD", "INR")).ReturnsAsync(83.1234m);

        var query = new CurrencyCompareQuery("US", "IN");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.ExchangeRate.Should().Be(83.1234m);
    }

    [Test]
    public async Task Handle_Should_RoundExchangeRate_ToFourDecimalPlaces()
    {
        // Arrange
        var homeCountry = CreateCountry("US", "United States", "USD");
        var destCountry = CreateCountry("GB", "United Kingdom", "GBP");

        _countryCacheMock.Setup(c => c.GetByCodeAsync("US")).ReturnsAsync(homeCountry);
        _countryCacheMock.Setup(c => c.GetByCodeAsync("GB")).ReturnsAsync(destCountry);
        _exchangeRateApiServiceMock.Setup(e => e.GetExchangeRateAsync("USD", "GBP")).ReturnsAsync(0.7912345678m);

        var query = new CurrencyCompareQuery("US", "GB");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.ExchangeRate.Should().Be(0.7912m);
    }

    #endregion

    #region Strength Hint Tests

    [Test]
    public async Task Handle_Should_ReturnStronger_WhenExchangeRateGreaterThanOne()
    {
        // Arrange
        var homeCountry = CreateCountry("US", "United States", "USD");
        var destCountry = CreateCountry("IN", "India", "INR");

        _countryCacheMock.Setup(c => c.GetByCodeAsync("US")).ReturnsAsync(homeCountry);
        _countryCacheMock.Setup(c => c.GetByCodeAsync("IN")).ReturnsAsync(destCountry);
        _exchangeRateApiServiceMock.Setup(e => e.GetExchangeRateAsync("USD", "INR")).ReturnsAsync(83m);

        var query = new CurrencyCompareQuery("US", "IN");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.StrengthHint.Should().Be("Your money is stronger");
    }

    [Test]
    public async Task Handle_Should_ReturnWeaker_WhenExchangeRateLessThanOne()
    {
        // Arrange
        var homeCountry = CreateCountry("IN", "India", "INR");
        var destCountry = CreateCountry("US", "United States", "USD");

        _countryCacheMock.Setup(c => c.GetByCodeAsync("IN")).ReturnsAsync(homeCountry);
        _countryCacheMock.Setup(c => c.GetByCodeAsync("US")).ReturnsAsync(destCountry);
        _exchangeRateApiServiceMock.Setup(e => e.GetExchangeRateAsync("INR", "USD")).ReturnsAsync(0.012m);

        var query = new CurrencyCompareQuery("IN", "US");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.StrengthHint.Should().Be("Your money is weaker");
    }

    [Test]
    public async Task Handle_Should_ReturnEqual_WhenExchangeRateIsOne()
    {
        // Arrange
        var homeCountry = CreateCountry("US", "United States", "USD");
        var destCountry = CreateCountry("PA", "Panama", "USD");

        _countryCacheMock.Setup(c => c.GetByCodeAsync("US")).ReturnsAsync(homeCountry);
        _countryCacheMock.Setup(c => c.GetByCodeAsync("PA")).ReturnsAsync(destCountry);
        _exchangeRateApiServiceMock.Setup(e => e.GetExchangeRateAsync("USD", "USD")).ReturnsAsync(1m);

        var query = new CurrencyCompareQuery("US", "PA");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.StrengthHint.Should().Be("Same currency");
    }

    #endregion

    #region Result Population Tests

    [Test]
    public async Task Handle_Should_PopulateAllResultFields()
    {
        // Arrange
        var homeCountry = CreateCountry("US", "United States", "USD");
        var destCountry = CreateCountry("GB", "United Kingdom", "GBP");

        _countryCacheMock.Setup(c => c.GetByCodeAsync("US")).ReturnsAsync(homeCountry);
        _countryCacheMock.Setup(c => c.GetByCodeAsync("GB")).ReturnsAsync(destCountry);
        _exchangeRateApiServiceMock.Setup(e => e.GetExchangeRateAsync("USD", "GBP")).ReturnsAsync(0.79m);

        var query = new CurrencyCompareQuery("US", "GB");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.HomeCountry.Should().Be("United States");
        result.HomeCurrency.Should().Be("USD");
        result.DestinationCountry.Should().Be("United Kingdom");
        result.DestinationCurrency.Should().Be("GBP");
        result.ExchangeRate.Should().Be(0.79m);
        result.StrengthHint.Should().Be("Your money is weaker");
    }

    #endregion

    private static CountryEntity CreateCountry(string code, string name, string currencyCode)
    {
        return new CountryEntity
        {
            Code = code,
            Name = name,
            CurrencyCode = currencyCode,
            CurrencyName = $"{currencyCode} Currency",
            CurrencySymbol = "$",
            FlagUrl = $"https://flags.example.com/{code}.png"
        };
    }
}
