using AwesomeAssertions;
using FluentValidation.TestHelper;
using NUnit.Framework;
using TravelGuideApi.Application.Features.Budget.Queries.Calculate;

namespace TravelGuideApi.Application.UnitTests.Features.Budget;

[TestFixture]
public class BudgetCalculateQueryValidatorTests
{
    private BudgetCalculateQueryValidator _validator = null!;

    [SetUp]
    public void Setup()
    {
        _validator = new BudgetCalculateQueryValidator();
    }

    #region Full Query Validation Tests

    [Test]
    public void Should_BeValid_When_QueryIsValid()
    {
        // Arrange
        var query = new BudgetCalculateQuery("USD", 100, 7, ["TH"]);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    #endregion

    #region HomeCurrency Validation Tests

    [Test]
    public void Should_HaveError_When_HomeCurrencyIsEmpty()
    {
        // Arrange
        var query = new BudgetCalculateQuery("", 100, 7, ["TH"]);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.HomeCurrency);
    }

    [Test]
    public void Should_HaveError_When_HomeCurrencyIsTooShort()
    {
        // Arrange
        var query = new BudgetCalculateQuery("US", 100, 7, ["TH"]);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.HomeCurrency)
            .WithErrorMessage("HomeCurrency must be a 3-character ISO currency code.");
    }

    [Test]
    public void Should_HaveError_When_HomeCurrencyIsTooLong()
    {
        // Arrange
        var query = new BudgetCalculateQuery("USDD", 100, 7, ["TH"]);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.HomeCurrency)
            .WithErrorMessage("HomeCurrency must be a 3-character ISO currency code.");
    }

    #endregion

    #region DailyBudget Validation Tests

    [Test]
    public void Should_HaveError_When_DailyBudgetIsZero()
    {
        // Arrange
        var query = new BudgetCalculateQuery("USD", 0, 7, ["TH"]);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DailyBudget)
            .WithErrorMessage("DailyBudget must be greater than 0.");
    }

    [Test]
    public void Should_HaveError_When_DailyBudgetIsNegative()
    {
        // Arrange
        var query = new BudgetCalculateQuery("USD", -50, 7, ["TH"]);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DailyBudget)
            .WithErrorMessage("DailyBudget must be greater than 0.");
    }

    #endregion

    #region TripDays Validation Tests

    [Test]
    public void Should_HaveError_When_TripDaysIsZero()
    {
        // Arrange
        var query = new BudgetCalculateQuery("USD", 100, 0, ["TH"]);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TripDays)
            .WithErrorMessage("TripDays must be greater than 0.");
    }

    [Test]
    public void Should_HaveError_When_TripDaysExceeds365()
    {
        // Arrange
        var query = new BudgetCalculateQuery("USD", 100, 366, ["TH"]);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TripDays)
            .WithErrorMessage("TripDays must not exceed 365 days.");
    }

    #endregion

    #region DestinationCountries Validation Tests

    [Test]
    public void Should_HaveError_When_DestinationCountriesIsEmpty()
    {
        // Arrange
        var query = new BudgetCalculateQuery("USD", 100, 7, []);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DestinationCountries)
            .WithErrorMessage("DestinationCountries must contain at least one country.");
    }

    [Test]
    public void Should_HaveError_When_CountryCodeIsTooShort()
    {
        // Arrange
        var query = new BudgetCalculateQuery("USD", 100, 7, ["T"]);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveAnyValidationError()
            .WithErrorMessage("Each destination country must be a 2-character ISO country code.");
    }

    [Test]
    public void Should_HaveError_When_CountryCodeIsTooLong()
    {
        // Arrange
        var query = new BudgetCalculateQuery("USD", 100, 7, ["THA"]);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveAnyValidationError()
            .WithErrorMessage("Each destination country must be a 2-character ISO country code.");
    }

    #endregion
}
