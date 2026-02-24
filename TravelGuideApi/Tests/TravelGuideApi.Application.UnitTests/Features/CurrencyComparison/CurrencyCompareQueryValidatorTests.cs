using AwesomeAssertions;
using FluentValidation.TestHelper;
using NUnit.Framework;
using TravelGuideApi.Application.Features.CurrencyComparison.Queries.Compare;

namespace TravelGuideApi.Application.UnitTests.Features.CurrencyComparison;

[TestFixture]
public class CurrencyCompareQueryValidatorTests
{
    private CurrencyCompareQueryValidator _validator = null!;

    [SetUp]
    public void Setup()
    {
        _validator = new CurrencyCompareQueryValidator();
    }

    #region HomeCountryCode Validation Tests

    [Test]
    public void Should_HaveError_When_HomeCountryCodeIsEmpty()
    {
        // Arrange
        var query = new CurrencyCompareQuery("", "GB");

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.HomeCountryCode);
    }

    [Test]
    public void Should_HaveError_When_HomeCountryCodeIsNull()
    {
        // Arrange
        var query = new CurrencyCompareQuery(null!, "GB");

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.HomeCountryCode);
    }

    [Test]
    public void Should_HaveError_When_HomeCountryCodeIsTooShort()
    {
        // Arrange
        var query = new CurrencyCompareQuery("U", "GB");

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.HomeCountryCode)
            .WithErrorMessage("Home Country Code must be a 2-character ISO country code.");
    }

    [Test]
    public void Should_HaveError_When_HomeCountryCodeIsTooLong()
    {
        // Arrange
        var query = new CurrencyCompareQuery("USA", "GB");

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.HomeCountryCode)
            .WithErrorMessage("Home Country Code must be a 2-character ISO country code.");
    }

    [Test]
    public void Should_NotHaveError_When_HomeCountryCodeIsValid()
    {
        // Arrange
        var query = new CurrencyCompareQuery("US", "GB");

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.HomeCountryCode);
    }

    #endregion

    #region DestinationCountryCode Validation Tests

    [Test]
    public void Should_HaveError_When_DestinationCountryCodeIsEmpty()
    {
        // Arrange
        var query = new CurrencyCompareQuery("US", "");

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DestinationCountryCode);
    }

    [Test]
    public void Should_HaveError_When_DestinationCountryCodeIsNull()
    {
        // Arrange
        var query = new CurrencyCompareQuery("US", null!);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DestinationCountryCode);
    }

    [Test]
    public void Should_HaveError_When_DestinationCountryCodeIsTooShort()
    {
        // Arrange
        var query = new CurrencyCompareQuery("US", "G");

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DestinationCountryCode)
            .WithErrorMessage("Destination Country Code must be a 2-character ISO country code.");
    }

    [Test]
    public void Should_HaveError_When_DestinationCountryCodeIsTooLong()
    {
        // Arrange
        var query = new CurrencyCompareQuery("US", "GBR");

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DestinationCountryCode)
            .WithErrorMessage("Destination Country Code must be a 2-character ISO country code.");
    }

    [Test]
    public void Should_NotHaveError_When_DestinationCountryCodeIsValid()
    {
        // Arrange
        var query = new CurrencyCompareQuery("US", "GB");

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.DestinationCountryCode);
    }

    #endregion

    #region Full Query Validation Tests

    [Test]
    public void Should_BeValid_When_BothCodesAreValid()
    {
        // Arrange
        var query = new CurrencyCompareQuery("US", "GB");

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Test]
    public void Should_BeInvalid_When_BothCodesAreEmpty()
    {
        // Arrange
        var query = new CurrencyCompareQuery("", "");

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCountGreaterThan(1);
    }

    [TestCase("us", "gb")]
    [TestCase("US", "GB")]
    [TestCase("Us", "Gb")]
    public void Should_BeValid_RegardlessOfCase(string homeCode, string destCode)
    {
        // Arrange
        var query = new CurrencyCompareQuery(homeCode, destCode);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    #endregion
}
