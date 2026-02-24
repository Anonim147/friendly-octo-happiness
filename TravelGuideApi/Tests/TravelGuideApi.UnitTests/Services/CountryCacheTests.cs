using AwesomeAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TravelGuideApi.Domain.Entities;
using TravelGuideApi.Infrastructure.Services;

namespace TravelGuideApi.UnitTests.Services;

[TestFixture]
public class CountryCacheTests
{
    private IMemoryCache _memoryCache = null!;
    private Mock<ILogger<CountryCache>> _loggerMock = null!;
    private CountryCache _cache = null!;

    [SetUp]
    public void Setup()
    {
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _loggerMock = new Mock<ILogger<CountryCache>>();
        _cache = new CountryCache(_memoryCache, _loggerMock.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _memoryCache.Dispose();
    }

    #region GetAllAsync Tests

    [Test]
    public async Task GetAllAsync_Should_ReturnNull_WhenCacheIsEmpty()
    {
        // Act
        var result = await _cache.GetAllAsync();

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task GetAllAsync_Should_ReturnCachedCountries_WhenCacheHasData()
    {
        // Arrange
        var countries = new List<CountryEntity>
        {
            new() { Code = "US", Name = "United States", CurrencyCode = "USD" },
            new() { Code = "GB", Name = "United Kingdom", CurrencyCode = "GBP" }
        };

        await _cache.SetAllAsync(countries);

        // Act
        var result = await _cache.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result!.First().Code.Should().Be("US");
    }

    #endregion

    #region SetAllAsync Tests

    [Test]
    public async Task SetAllAsync_Should_CacheCountries()
    {
        // Arrange
        var countries = new List<CountryEntity>
        {
            new() { Code = "FR", Name = "France", CurrencyCode = "EUR" }
        };

        // Act
        await _cache.SetAllAsync(countries);

        // Assert
        var result = await _cache.GetAllAsync();
        result.Should().NotBeNull();
        result!.First().Code.Should().Be("FR");
    }

    [Test]
    public async Task SetAllAsync_Should_OverwriteExistingCache()
    {
        // Arrange
        var initialCountries = new List<CountryEntity>
        {
            new() { Code = "US", Name = "United States", CurrencyCode = "USD" }
        };

        var newCountries = new List<CountryEntity>
        {
            new() { Code = "GB", Name = "United Kingdom", CurrencyCode = "GBP" },
            new() { Code = "FR", Name = "France", CurrencyCode = "EUR" }
        };

        await _cache.SetAllAsync(initialCountries);

        // Act
        await _cache.SetAllAsync(newCountries);

        // Assert
        var result = await _cache.GetAllAsync();
        result.Should().HaveCount(2);
        result!.Any(c => c.Code == "US").Should().BeFalse();
        result.Any(c => c.Code == "GB").Should().BeTrue();
    }

    [Test]
    public async Task SetAllAsync_Should_HandleEmptyList()
    {
        // Arrange
        var emptyList = new List<CountryEntity>();

        // Act
        await _cache.SetAllAsync(emptyList);

        // Assert
        var result = await _cache.GetAllAsync();
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    #endregion

    #region GetByCodeAsync Tests

    [Test]
    public async Task GetByCodeAsync_Should_ReturnNull_WhenCacheIsEmpty()
    {
        // Act
        var result = await _cache.GetByCodeAsync("US");

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task GetByCodeAsync_Should_ReturnCountry_WhenFound()
    {
        // Arrange
        var countries = new List<CountryEntity>
        {
            new() { Code = "US", Name = "United States", CurrencyCode = "USD" },
            new() { Code = "GB", Name = "United Kingdom", CurrencyCode = "GBP" }
        };

        await _cache.SetAllAsync(countries);

        // Act
        var result = await _cache.GetByCodeAsync("US");

        // Assert
        result.Should().NotBeNull();
        result!.Code.Should().Be("US");
        result.Name.Should().Be("United States");
    }

    [Test]
    public async Task GetByCodeAsync_Should_ReturnNull_WhenCountryNotFound()
    {
        // Arrange
        var countries = new List<CountryEntity>
        {
            new() { Code = "US", Name = "United States", CurrencyCode = "USD" }
        };

        await _cache.SetAllAsync(countries);

        // Act
        var result = await _cache.GetByCodeAsync("XX");

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task GetByCodeAsync_Should_BeCaseInsensitive()
    {
        // Arrange
        var countries = new List<CountryEntity>
        {
            new() { Code = "US", Name = "United States", CurrencyCode = "USD" }
        };

        await _cache.SetAllAsync(countries);

        // Act
        var resultLower = await _cache.GetByCodeAsync("us");
        var resultUpper = await _cache.GetByCodeAsync("US");
        var resultMixed = await _cache.GetByCodeAsync("Us");

        // Assert
        resultLower.Should().NotBeNull();
        resultUpper.Should().NotBeNull();
        resultMixed.Should().NotBeNull();
    }

    #endregion
}
