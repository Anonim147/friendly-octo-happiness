using AwesomeAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TravelGuideApi.Application.Features.Country.Queries.GetAll;
using TravelGuideApi.Application.Interfaces;
using TravelGuideApi.Domain.Entities;
using TravelGuideApi.Domain.Interfaces;

namespace TravelGuideApi.Application.UnitTests.Features.Country;

[TestFixture]
public class CountryGetAllQueryHandlerTests
{
    private Mock<ICountryApiService> _countryApiServiceMock = null!;
    private Mock<ICountryCache> _countryCacheMock = null!;
    private Mock<ILogger<CountryGetAllQueryHandler>> _loggerMock = null!;
    private CountryGetAllQueryHandler _handler = null!;

    [SetUp]
    public void Setup()
    {
        _countryApiServiceMock = new Mock<ICountryApiService>();
        _countryCacheMock = new Mock<ICountryCache>();
        _loggerMock = new Mock<ILogger<CountryGetAllQueryHandler>>();

        _handler = new CountryGetAllQueryHandler(
            _countryApiServiceMock.Object,
            _countryCacheMock.Object,
            _loggerMock.Object);
    }

    [Test]
    public async Task Handle_Should_ReturnCachedCountries_WhenCacheHit()
    {
        // Arrange
        var cachedCountries = new List<CountryEntity>
        {
            new() { Code = "US", Name = "United States", CurrencyCode = "USD" },
            new() { Code = "GB", Name = "United Kingdom", CurrencyCode = "GBP" }
        };

        _countryCacheMock
            .Setup(c => c.GetAllAsync())
            .ReturnsAsync(cachedCountries);

        var query = new CountryGetAllQuery();

        // Act
        IEnumerable<CountryEntity> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(cachedCountries);
        _countryApiServiceMock.Verify(s => s.GetAllCountriesAsync(), Times.Never);
    }

    [Test]
    public async Task Handle_Should_CacheApiResults_WhenCacheMiss()
    {
        // Arrange
        var apiCountries = new List<CountryEntity>
        {
            new() { Code = "US", Name = "United States", CurrencyCode = "USD" }
        };

        _countryCacheMock
            .Setup(c => c.GetAllAsync())
            .ReturnsAsync((IEnumerable<CountryEntity>?)null);

        _countryApiServiceMock
            .Setup(s => s.GetAllCountriesAsync())
            .ReturnsAsync(apiCountries);

        var query = new CountryGetAllQuery();

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _countryCacheMock.Verify(c => c.SetAllAsync(It.Is<IEnumerable<CountryEntity>>(
            countries => countries.Count() == 1 && countries.First().Code == "US")), Times.Once);
    }
}
