using AwesomeAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TravelGuideApi.Application.Features.Budget.Queries.Calculate;
using TravelGuideApi.Application.Interfaces;
using TravelGuideApi.Domain.Entities;

namespace TravelGuideApi.Application.UnitTests.Features.Budget;

[TestFixture]
public class BudgetCalculateQueryHandlerTests
{
    private Mock<IExchangeRateCache> _exchangeRateCacheMock = null!;
    private Mock<ICountryCache> _countryCacheMock = null!;
    private Mock<ILogger<BudgetCalculateQueryHandler>> _loggerMock = null!;
    private BudgetCalculateQueryHandler _handler = null!;

    [SetUp]
    public void Setup()
    {
        _exchangeRateCacheMock = new Mock<IExchangeRateCache>();
        _countryCacheMock = new Mock<ICountryCache>();
        _loggerMock = new Mock<ILogger<BudgetCalculateQueryHandler>>();

        _handler = new BudgetCalculateQueryHandler(
            _exchangeRateCacheMock.Object,
            _countryCacheMock.Object,
            _loggerMock.Object);
    }

    #region Happy Path Tests

    [Test]
    public async Task Handle_SingleDestination_ReturnsCorrectBudgetCalculationResult()
    {
        // Arrange
        var gbCountry = CreateCountry("GB", "United Kingdom", "GBP");
        var rates = new Dictionary<string, decimal> { ["GBP"] = 0.79m };

        _exchangeRateCacheMock.Setup(c => c.GetRatesAsync("USD")).ReturnsAsync(rates);
        _countryCacheMock.Setup(c => c.GetByCodeAsync("GB")).ReturnsAsync(gbCountry);

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

        var rates = new Dictionary<string, decimal>
        {
            ["GBP"] = 0.79m,
            ["JPY"] = 149.5m,
            ["INR"] = 83.12m
        };

        _exchangeRateCacheMock.Setup(c => c.GetRatesAsync("USD")).ReturnsAsync(rates);
        _countryCacheMock.Setup(c => c.GetByCodeAsync("GB")).ReturnsAsync(gbCountry);
        _countryCacheMock.Setup(c => c.GetByCodeAsync("JP")).ReturnsAsync(jpCountry);
        _countryCacheMock.Setup(c => c.GetByCodeAsync("IN")).ReturnsAsync(inCountry);

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

        var rates = new Dictionary<string, decimal>
        {
            ["GBP"] = 0.79m,
            ["JPY"] = 149.5m,
            ["INR"] = 83.12m
        };

        _exchangeRateCacheMock.Setup(c => c.GetRatesAsync("USD")).ReturnsAsync(rates);
        _countryCacheMock.Setup(c => c.GetByCodeAsync("GB")).ReturnsAsync(gbCountry);
        _countryCacheMock.Setup(c => c.GetByCodeAsync("JP")).ReturnsAsync(jpCountry);
        _countryCacheMock.Setup(c => c.GetByCodeAsync("IN")).ReturnsAsync(inCountry);

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
    public async Task Handle_MissingRateForDestinationCurrency_ThrowsInvalidOperationException()
    {
        // Arrange
        var frCountry = CreateCountry("FR", "France", "EUR");
        var rates = new Dictionary<string, decimal> { ["GBP"] = 0.79m }; // EUR not included

        _exchangeRateCacheMock.Setup(c => c.GetRatesAsync("USD")).ReturnsAsync(rates);
        _countryCacheMock.Setup(c => c.GetByCodeAsync("FR")).ReturnsAsync(frCountry);

        var query = new BudgetCalculateQuery("USD", 100m, 5, ["FR"]);

        // Act
        Func<Task> act = () => _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*EUR*");
    }

    [Test]
    public async Task Handle_NullRatesFromCache_ThrowsInvalidOperationException()
    {
        // Arrange
        _exchangeRateCacheMock.Setup(c => c.GetRatesAsync("USD")).ReturnsAsync((Dictionary<string, decimal>?)null);

        var query = new BudgetCalculateQuery("USD", 100m, 5, ["GB"]);

        // Act
        Func<Task> act = () => _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*USD*");
    }

    [Test]
    public async Task Handle_UnknownCountryCode_ThrowsArgumentException()
    {
        // Arrange
        var rates = new Dictionary<string, decimal> { ["GBP"] = 0.79m };

        _exchangeRateCacheMock.Setup(c => c.GetRatesAsync("USD")).ReturnsAsync(rates);
        _countryCacheMock.Setup(c => c.GetByCodeAsync("XX")).ReturnsAsync((CountryEntity?)null);

        var query = new BudgetCalculateQuery("USD", 100m, 5, ["XX"]);

        // Act
        Func<Task> act = () => _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*XX*");
    }

    #endregion

    #region DataFreshness and Timestamp Tests

    [Test]
    public async Task Handle_Result_HasLiveDataFreshness()
    {
        // Arrange
        var gbCountry = CreateCountry("GB", "United Kingdom", "GBP");
        var rates = new Dictionary<string, decimal> { ["GBP"] = 0.79m };

        _exchangeRateCacheMock.Setup(c => c.GetRatesAsync("USD")).ReturnsAsync(rates);
        _countryCacheMock.Setup(c => c.GetByCodeAsync("GB")).ReturnsAsync(gbCountry);

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
        var rates = new Dictionary<string, decimal> { ["GBP"] = 0.79m };

        _exchangeRateCacheMock.Setup(c => c.GetRatesAsync("USD")).ReturnsAsync(rates);
        _countryCacheMock.Setup(c => c.GetByCodeAsync("GB")).ReturnsAsync(gbCountry);

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
        var rates = new Dictionary<string, decimal> { ["GBP"] = 0.7912345678m };

        _exchangeRateCacheMock.Setup(c => c.GetRatesAsync("USD")).ReturnsAsync(rates);
        _countryCacheMock.Setup(c => c.GetByCodeAsync("GB")).ReturnsAsync(gbCountry);

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
