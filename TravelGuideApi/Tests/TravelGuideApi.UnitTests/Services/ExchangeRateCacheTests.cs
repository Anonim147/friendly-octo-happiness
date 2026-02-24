using AwesomeAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using TravelGuideApi.Infrastructure.Services;

namespace TravelGuideApi.UnitTests.Services;

[TestFixture]
public class ExchangeRateCacheTests
{
    private IMemoryCache _memoryCache = null!;
    private Mock<ILogger<ExchangeRateCache>> _loggerMock = null!;
    private ExchangeRateCache _cache = null!;

    [SetUp]
    public void Setup()
    {
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _loggerMock = new Mock<ILogger<ExchangeRateCache>>();
        _cache = new ExchangeRateCache(_memoryCache, _loggerMock.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _memoryCache.Dispose();
    }

    #region GetRatesAsync Tests

    [Test]
    public async Task GetRatesAsync_Should_ReturnNull_WhenCacheIsEmpty()
    {
        // Act
        var result = await _cache.GetRatesAsync("USD");

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task GetRatesAsync_Should_ReturnCachedRates_WhenCacheHasData()
    {
        // Arrange
        var rates = new Dictionary<string, decimal>
        {
            { "GBP", 0.79m },
            { "EUR", 0.92m },
            { "INR", 83.12m }
        };

        await _cache.SetRatesAsync("USD", rates);

        // Act
        var result = await _cache.GetRatesAsync("USD");

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result!["GBP"].Should().Be(0.79m);
    }

    [Test]
    public async Task GetRatesAsync_Should_ReturnNull_ForDifferentBaseCurrency()
    {
        // Arrange
        var rates = new Dictionary<string, decimal>
        {
            { "USD", 1.26m }
        };

        await _cache.SetRatesAsync("GBP", rates);

        // Act
        var result = await _cache.GetRatesAsync("USD");

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task GetRatesAsync_Should_BeCaseInsensitive()
    {
        // Arrange
        var rates = new Dictionary<string, decimal>
        {
            { "GBP", 0.79m }
        };

        await _cache.SetRatesAsync("USD", rates);

        // Act
        var resultLower = await _cache.GetRatesAsync("usd");
        var resultUpper = await _cache.GetRatesAsync("USD");
        var resultMixed = await _cache.GetRatesAsync("Usd");

        // Assert
        resultLower.Should().NotBeNull();
        resultUpper.Should().NotBeNull();
        resultMixed.Should().NotBeNull();
    }

    #endregion

    #region SetRatesAsync Tests

    [Test]
    public async Task SetRatesAsync_Should_CacheRates()
    {
        // Arrange
        var rates = new Dictionary<string, decimal>
        {
            { "EUR", 0.92m }
        };

        // Act
        await _cache.SetRatesAsync("USD", rates);

        // Assert
        var result = await _cache.GetRatesAsync("USD");
        result.Should().NotBeNull();
        result!["EUR"].Should().Be(0.92m);
    }

    [Test]
    public async Task SetRatesAsync_Should_OverwriteExistingCache()
    {
        // Arrange
        var initialRates = new Dictionary<string, decimal>
        {
            { "GBP", 0.79m }
        };

        var newRates = new Dictionary<string, decimal>
        {
            { "GBP", 0.80m },
            { "EUR", 0.93m }
        };

        await _cache.SetRatesAsync("USD", initialRates);

        // Act
        await _cache.SetRatesAsync("USD", newRates);

        // Assert
        var result = await _cache.GetRatesAsync("USD");
        result.Should().HaveCount(2);
        result!["GBP"].Should().Be(0.80m);
        result["EUR"].Should().Be(0.93m);
    }

    [Test]
    public async Task SetRatesAsync_Should_HandleEmptyRates()
    {
        // Arrange
        var emptyRates = new Dictionary<string, decimal>();

        // Act
        await _cache.SetRatesAsync("USD", emptyRates);

        // Assert
        var result = await _cache.GetRatesAsync("USD");
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Test]
    public async Task SetRatesAsync_Should_HandleMultipleCurrencies()
    {
        // Arrange
        var usdRates = new Dictionary<string, decimal>
        {
            { "GBP", 0.79m }
        };

        var gbpRates = new Dictionary<string, decimal>
        {
            { "USD", 1.26m }
        };

        // Act
        await _cache.SetRatesAsync("USD", usdRates);
        await _cache.SetRatesAsync("GBP", gbpRates);

        // Assert
        var usdResult = await _cache.GetRatesAsync("USD");
        var gbpResult = await _cache.GetRatesAsync("GBP");

        usdResult.Should().NotBeNull();
        usdResult!["GBP"].Should().Be(0.79m);
        
        gbpResult.Should().NotBeNull();
        gbpResult!["USD"].Should().Be(1.26m);
    }

    #endregion

    #region Case Sensitivity Tests for Currency Codes

    [Test]
    public async Task SetRatesAsync_Should_NormalizeBaseCurrencyToUpperCase()
    {
        // Arrange
        var rates = new Dictionary<string, decimal>
        {
            { "GBP", 0.79m }
        };

        // Act
        await _cache.SetRatesAsync("usd", rates);

        // Assert
        var result = await _cache.GetRatesAsync("USD");
        result.Should().NotBeNull();
    }

    #endregion
}
