using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AwesomeAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using TravelGuideApi.Application.Features.ExchangeRateHistory.Queries;
using TravelGuideApi.Domain.Interfaces;
using TravelGuideApi.Domain.Models;

namespace TravelGuideApi.Application.UnitTests.Features.ExchangeRateHistory;

[TestFixture]
public class ExchangeRateHistoryQueryHandlerTests
{
    private Mock<IExchangeRateHistoryService> _historyServiceMock = null!;
    private Mock<ILogger<ExchangeRateHistoryQueryHandler>> _loggerMock = null!;
    private ExchangeRateHistoryQueryHandler _handler = null!;

    [SetUp]
    public void Setup()
    {
        _historyServiceMock = new Mock<IExchangeRateHistoryService>();
        _loggerMock = new Mock<ILogger<ExchangeRateHistoryQueryHandler>>();

        _handler = new ExchangeRateHistoryQueryHandler(
            _historyServiceMock.Object,
            _loggerMock.Object);
    }

    [Test]
    public async Task Handle_Should_CallHistoryService_WithCorrectDateRange()
    {
        // Arrange
        var query = new ExchangeRateHistoryQuery("USD", "EUR", 30);
        var expectedDataPoints = new List<ExchangeRateDataPoint>
        {
            new() { Date = new DateOnly(2024, 1, 1), Rate = 0.92m },
            new() { Date = new DateOnly(2024, 1, 2), Rate = 0.91m }
        };

        _historyServiceMock
            .Setup(s => s.GetHistoricalRatesAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<DateOnly>(),
                It.IsAny<DateOnly>()))
            .ReturnsAsync(expectedDataPoints.AsReadOnly());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        _historyServiceMock.Verify(
            s => s.GetHistoricalRatesAsync(
                "USD",
                "EUR",
                It.IsAny<DateOnly>(),
                It.IsAny<DateOnly>()),
            Times.Once);
    }

    [Test]
    public async Task Handle_Should_PassCorrectCurrencies_ToHistoryService()
    {
        // Arrange
        var query = new ExchangeRateHistoryQuery("GBP", "JPY", 90);
        _historyServiceMock
            .Setup(s => s.GetHistoricalRatesAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<DateOnly>(),
                It.IsAny<DateOnly>()))
            .ReturnsAsync(Array.Empty<ExchangeRateDataPoint>());

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _historyServiceMock.Verify(
            s => s.GetHistoricalRatesAsync(
                "GBP",
                "JPY",
                It.IsAny<DateOnly>(),
                It.IsAny<DateOnly>()),
            Times.Once);
    }

    [Test]
    public async Task Handle_Should_PassCorrectDateRange_For30Days()
    {
        // Arrange
        var query = new ExchangeRateHistoryQuery("USD", "EUR", 30);
        DateOnly capturedStart = default;
        DateOnly capturedEnd = default;

        _historyServiceMock
            .Setup(s => s.GetHistoricalRatesAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<DateOnly>(),
                It.IsAny<DateOnly>()))
            .Callback<string, string, DateOnly, DateOnly>((_, _, start, end) =>
            {
                capturedStart = start;
                capturedEnd = end;
            })
            .ReturnsAsync(Array.Empty<ExchangeRateDataPoint>());

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        capturedEnd.Should().Be(today);
        capturedStart.Should().Be(today.AddDays(-30));
    }

    [Test]
    public async Task Handle_Should_PassCorrectDateRange_For365Days()
    {
        // Arrange
        var query = new ExchangeRateHistoryQuery("USD", "EUR", 365);
        DateOnly capturedStart = default;
        DateOnly capturedEnd = default;

        _historyServiceMock
            .Setup(s => s.GetHistoricalRatesAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<DateOnly>(),
                It.IsAny<DateOnly>()))
            .Callback<string, string, DateOnly, DateOnly>((_, _, start, end) =>
            {
                capturedStart = start;
                capturedEnd = end;
            })
            .ReturnsAsync(Array.Empty<ExchangeRateDataPoint>());

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        capturedEnd.Should().Be(today);
        capturedStart.Should().Be(today.AddDays(-365));
    }

    [Test]
    public async Task Handle_Should_ReturnEmptyList_WhenServiceReturnsNoData()
    {
        // Arrange
        var query = new ExchangeRateHistoryQuery("USD", "EUR", 30);
        _historyServiceMock
            .Setup(s => s.GetHistoricalRatesAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<DateOnly>(),
                It.IsAny<DateOnly>()))
            .ReturnsAsync(Array.Empty<ExchangeRateDataPoint>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }

    [Test]
    public async Task Handle_Should_ReturnDataPoints_InOrderReturnedByService()
    {
        // Arrange
        var query = new ExchangeRateHistoryQuery("USD", "EUR", 30);
        var dataPoints = new List<ExchangeRateDataPoint>
        {
            new() { Date = new DateOnly(2024, 1, 1), Rate = 0.92m },
            new() { Date = new DateOnly(2024, 1, 2), Rate = 0.91m },
            new() { Date = new DateOnly(2024, 1, 3), Rate = 0.93m }
        };

        _historyServiceMock
            .Setup(s => s.GetHistoricalRatesAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<DateOnly>(),
                It.IsAny<DateOnly>()))
            .ReturnsAsync(dataPoints.AsReadOnly());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(3);
        result[0].Rate.Should().Be(0.92m);
        result[1].Rate.Should().Be(0.91m);
        result[2].Rate.Should().Be(0.93m);
    }
}
