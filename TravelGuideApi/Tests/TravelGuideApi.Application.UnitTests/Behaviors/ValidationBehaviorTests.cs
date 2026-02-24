using AwesomeAssertions;
using FluentValidation;
using TravelGuideApi.Application.Behaviors;
using TravelGuideApi.Application.Features.CurrencyComparison.Queries.Compare;
using TravelGuideApi.Domain.Models;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TravelGuideApi.Application.UnitTests.Behaviors;

[TestFixture]
public class ValidationBehaviorTests
{
    [Test]
    public async Task Should_ThrowValidationException_When_QueryIsNotValid()
    {
        // Arrange
        CurrencyCompareQuery query = new("", ""); // Empty country codes are invalid

        ValidationBehavior<CurrencyCompareQuery, CurrencyComparisonResult> validationBehavior = new(
            [
                new CurrencyCompareQueryValidator()
            ]
        );

        // Act
        Func<Task> act = () => validationBehavior.Handle(
            query,
            () => Task.FromResult(new CurrencyComparisonResult()),
            CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task Should_ExecuteHandler_When_QueryIsValid()
    {
        // Arrange
        CurrencyCompareQuery query = new("US", "GB"); // Valid 2-character country codes

        ValidationBehavior<CurrencyCompareQuery, CurrencyComparisonResult> validationBehavior = new(
            [
                new CurrencyCompareQueryValidator()
            ]
        );

        // Act
        Func<Task> act = () => validationBehavior.Handle(
            query,
            () => Task.FromResult(new CurrencyComparisonResult()),
            CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync<ValidationException>();
    }
}
