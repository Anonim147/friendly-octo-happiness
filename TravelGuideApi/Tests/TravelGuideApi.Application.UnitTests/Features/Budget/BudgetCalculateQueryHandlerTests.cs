using AwesomeAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using TravelGuideApi.Application.Features.Budget.Queries.Calculate;
using TravelGuideApi.Application.Interfaces;
using TravelGuideApi.Domain.Entities;
using TravelGuideApi.Domain.Interfaces;

namespace TravelGuideApi.Application.UnitTests.Features.Budget;

[TestFixture]
public class BudgetCalculateQueryHandlerTests
{
    private Mock<IExchangeRateApiService> _exchangeRateApiServiceMock = null!;
    private Mock<ICountryApiService> _countryApiServiceMock = null!;
    private Mock<ICountryCache> _countryCacheMock = null!;
    private Mock<ILogger<BudgetCalculateQueryHandler>> _loggerMock = null!;
    private BudgetCalculateQueryHandler _handler = null!;

    [SetUp]
    public void Setup()
    {
        _exchangeRateApiServiceMock = new Mock<IExchangeRateApiService>();
        _countryApiServiceMock = new Mock<ICountryApiService>();
        _countryCacheMock = new Mock<ICountryCache>();
        _loggerMock = new Mock<ILogger<BudgetCalculateQueryHandler>>();

        _handler = new BudgetCalculateQueryHandler(
            _exchangeRateApiServiceMock.Object,
            _countryApiServiceMock.Object,
            _countryCacheMock.Object,
            _loggerMock.Object);
    }

    #region Happy Path Tests

    [Test]
    public async Task Handle_SingleDestination_ReturnsCorrectBudgetCalculationResult()
    {
        // Arrange
        var gbCountry = CreateCountry("GB", "United Kingdom", "GBP");

        _countryCacheMock.Setup(c => c.GetByCodeAsync("GB")).ReturnsAsync(gbCountry);
        _exchangeRateApiServiceMock.Setup(s => s.GetExchangeRateAsync("USD", "GBP")).ReturnsAsync(0.79m);

        var query = new BudgetCalculateQuery("USD", 100m, 7, ["GB"]);

        // Act
        var results = await _handler.Handle(query, CancellationToken.None);

        // Assert
        results.Should().HaveCount(1);

        var result = results[0];
        result.CountryCode.Should().Be("GB");
        result.CountryName.Should().Be("United Kingdom");
        result.DestinationCurrency.Should().Be("GBP");
        result.ExchangeRate.Should().Be(0.79m);
        result.DailyBudgetHome.Should().Be(100m);
        result.TripDays.Should().Be(7);
        result.TotalBudgetHome.Should().Be(700m);
        result.TotalBudgetLocal.Should().Be(553m);    // 700 * 0.79
        result.DailyLocalAmount.Should().Be(79m);     // 100 * 0.79
    }

    [Test]
    public async Task Handle_MultiDestination_ReturnsResultForEachCountry()
    {
        // Arrange
        var gbCountry = CreateCountry("GB", "United Kingdom", "GBP");
        var jpCountry = CreateCountry("JP", "Japan", "JPY");
        var inCountry = CreateCountry("IN", "India", "INR");

        _countryCacheMock.Setup(c => c.GetByCodeAsync("GB")).ReturnsAsync(gbCountry);
        _countryCacheMock.Setup(c => c.GetByCodeAsync("JP")).ReturnsAsync(jpCountry);
        _countryCacheMock.Setup(c => c.GetByCodeAsync("IN")).ReturnsAsync(inCountry);
        _exchangeRateApiServiceMock.Setup(s => s.GetExchangeRateAsync("USD", "GBP")).ReturnsAsync(0.79m);
        _exchangeRateApiServiceMock.Setup(s => s.GetExchangeRateAsync("USD", "JPY")).ReturnsAsync(149.5m);
        _exchangeRateApiServiceMock.Setup(s => s.GetExchangeRateAsync("USD", "INR")).ReturnsAsync(83.12m);

        var query = new BudgetCalculateQuery("USD", 50m, 10, ["GB", "JP", "IN"]);

        // Act
        var results = await _handler.Handle(query, CancellationToken.None);

        // Assert
        results.Should().HaveCount(3);
        results.Should().Contain(r => r.CountryCode == "GB" && r.DestinationCurrency == "GBP");
        results.Should().Contain(r => r.CountryCode == "JP" && r.DestinationCurrency == "JPY");
        results.Should().Contain(r => r.CountryCode == "IN" && r.DestinationCurrency == "INR");
    }

    [Test]
    public async Task Handle_MultiDestination_CalculatesMathCorrectlyForEachCountry()
    {
        // Arrange
        var gbCountry = CreateCountry("GB", "United Kingdom", "GBP");
        var jpCountry = CreateCountry("JP", "Japan", "JPY");
        var inCountry = CreateCountry("IN", "India", "INR");

        _countryCacheMock.Setup(c => c.GetByCodeAsync("GB")).ReturnsAsync(gbCountry);
        _countryCacheMock.Setup(c => c.GetByCodeAsync("JP")).ReturnsAsync(jpCountry);
        _countryCacheMock.Setup(c => c.GetByCodeAsync("IN")).ReturnsAsync(inCountry);
        _exchangeRateApiServiceMock.Setup(s => s.GetExchangeRateAsync("USD", "GBP")).ReturnsAsync(0.79m);
        _exchangeRateApiServiceMock.Setup(s => s.GetExchangeRateAsync("USD", "JPY")).ReturnsAsync(149.5m);
        _exchangeRateApiServiceMock.Setup(s => s.GetExchangeRateAsync("USD", "INR")).ReturnsAsync(83.12m);

        var query = new BudgetCalculateQuery("USD", 50m, 10, ["GB", "JP", "IN"]);

        // Act
        var results = await _handler.Handle(query, CancellationToken.None);

        // Assert
        var gbResult = results.Find(r => r.CountryCode == "GB")!;
        gbResult.TotalBudgetHome.Should().Be(500m);
        gbResult.TotalBudgetLocal.Should().Be(395m);   // 500 * 0.79
        gbResult.DailyLocalAmount.Should().Be(39.5m);  // 50 * 0.79

        var jpResult = results.Find(r => r.CountryCode == "JP")!;
        jpResult.TotalBudgetLocal.Should().Be(74750m); // 500 * 149.5
        jpResult.DailyLocalAmount.Should().Be(7475m);  // 50 * 149.5

        var inResult = results.Find(r => r.CountryCode == "IN")!;
        inResult.TotalBudgetLocal.Should().Be(41560m); // 500 * 83.12
        inResult.DailyLocalAmount.Should().Be(4156m);  // 50 * 83.12
    }

    #endregion

    #region Error Tests

    [Test]
    public async Task Handle_UnknownCountryCode_ThrowsArgumentException()
    {
        // Arrange
        _countryCacheMock.Setup(c => c.GetByCodeAsync("XX")).ReturnsAsync((CountryEntity?)null);
        _countryApiServiceMock.Setup(s => s.GetCountryByCodeAsync("XX")).ReturnsAsync((CountryEntity?)null);

        var query = new BudgetCalculateQuery("USD", 100m, 5, ["XX"]);

        // Act
        Func<Task> act = () => _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*XX*");
    }

    [Test]
    public async Task Handle_CountryCacheMiss_FallsBackToApiService()
    {
        // Arrange
        var gbCountry = CreateCountry("GB", "United Kingdom", "GBP");

        _countryCacheMock.Setup(c => c.GetByCodeAsync("GB")).ReturnsAsync((CountryEntity?)null);
        _countryApiServiceMock.Setup(s => s.GetCountryByCodeAsync("GB")).ReturnsAsync(gbCountry);
        _exchangeRateApiServiceMock.Setup(s => s.GetExchangeRateAsync("USD", "GBP")).ReturnsAsync(0.79m);

        var query = new BudgetCalculateQuery("USD", 100m, 7, ["GB"]);

        // Act
        var results = await _handler.Handle(query, CancellationToken.None);

        // Assert
        results.Should().HaveCount(1);
        results[0].CountryCode.Should().Be("GB");
        _countryApiServiceMock.Verify(s => s.GetCountryByCodeAsync("GB"), Times.Once);
    }

    #endregion

    #region DataFreshness and Timestamp Tests

    [Test]
    public async Task Handle_Result_HasLiveDataFreshness()
    {
        // Arrange
        var gbCountry = CreateCountry("GB", "United Kingdom", "GBP");

        _countryCacheMock.Setup(c => c.GetByCodeAsync("GB")).ReturnsAsync(gbCountry);
        _exchangeRateApiServiceMock.Setup(s => s.GetExchangeRateAsync("USD", "GBP")).ReturnsAsync(0.79m);

        var query = new BudgetCalculateQuery("USD", 100m, 7, ["GB"]);

        // Act
        var results = await _handler.Handle(query, CancellationToken.None);

        // Assert
        results[0].DataFreshness.Should().Be("Live");
    }

    [Test]
    public async Task Handle_Result_HasRecentRateTimestamp()
    {
        // Arrange
        var gbCountry = CreateCountry("GB", "United Kingdom", "GBP");

        _countryCacheMock.Setup(c => c.GetByCodeAsync("GB")).ReturnsAsync(gbCountry);
        _exchangeRateApiServiceMock.Setup(s => s.GetExchangeRateAsync("USD", "GBP")).ReturnsAsync(0.79m);

        var query = new BudgetCalculateQuery("USD", 100m, 7, ["GB"]);
        var beforeCall = DateTime.UtcNow;

        // Act
        var results = await _handler.Handle(query, CancellationToken.None);

        // Assert
        var afterCall = DateTime.UtcNow;
        results[0].RateTimestamp.Should().BeOnOrAfter(beforeCall);
        results[0].RateTimestamp.Should().BeOnOrBefore(afterCall.AddSeconds(5));
    }

    #endregion

    #region Exchange Rate Rounding Tests

    [Test]
    public async Task Handle_ExchangeRate_IsRoundedToFourDecimalPlaces()
    {
        // Arrange
        var gbCountry = CreateCountry("GB", "United Kingdom", "GBP");

        _countryCacheMock.Setup(c => c.GetByCodeAsync("GB")).ReturnsAsync(gbCountry);
        _exchangeRateApiServiceMock.Setup(s => s.GetExchangeRateAsync("USD", "GBP")).ReturnsAsync(0.7912345678m);

        var query = new BudgetCalculateQuery("USD", 100m, 7, ["GB"]);

        // Act
        var results = await _handler.Handle(query, CancellationToken.None);

        // Assert
        results[0].ExchangeRate.Should().Be(0.7912m);
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
