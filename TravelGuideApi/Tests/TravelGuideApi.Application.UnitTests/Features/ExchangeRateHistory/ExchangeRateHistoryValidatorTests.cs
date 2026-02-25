using AwesomeAssertions;
using FluentValidation.TestHelper;
using NUnit.Framework;
using TravelGuideApi.Application.Features.ExchangeRateHistory.Queries;

namespace TravelGuideApi.Application.UnitTests.Features.ExchangeRateHistory;

[TestFixture]
public class ExchangeRateHistoryValidatorTests
{
    private ExchangeRateHistoryValidator _validator = null!;

    [SetUp]
    public void Setup()
    {
        _validator = new ExchangeRateHistoryValidator();
    }

    #region FromCurrency Validation Tests

    [Test]
    public void Should_HaveError_When_FromCurrencyIsEmpty()
    {
        var query = new ExchangeRateHistoryQuery("", "EUR", 30);
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.FromCurrency);
    }

    [Test]
    public void Should_HaveError_When_FromCurrencyIsNull()
    {
        var query = new ExchangeRateHistoryQuery(null!, "EUR", 30);
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.FromCurrency);
    }

    [Test]
    public void Should_HaveError_When_FromCurrencyIsTooShort()
    {
        var query = new ExchangeRateHistoryQuery("US", "EUR", 30);
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.FromCurrency)
            .WithErrorMessage("From Currency must be a 3-character ISO currency code.");
    }

    [Test]
    public void Should_HaveError_When_FromCurrencyIsTooLong()
    {
        var query = new ExchangeRateHistoryQuery("USDD", "EUR", 30);
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.FromCurrency)
            .WithErrorMessage("From Currency must be a 3-character ISO currency code.");
    }

    [Test]
    public void Should_NotHaveError_When_FromCurrencyIsValid()
    {
        var query = new ExchangeRateHistoryQuery("USD", "EUR", 30);
        var result = _validator.TestValidate(query);
        result.ShouldNotHaveValidationErrorFor(x => x.FromCurrency);
    }

    #endregion

    #region ToCurrency Validation Tests

    [Test]
    public void Should_HaveError_When_ToCurrencyIsEmpty()
    {
        var query = new ExchangeRateHistoryQuery("USD", "", 30);
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.ToCurrency);
    }

    [Test]
    public void Should_HaveError_When_ToCurrencyIsNull()
    {
        var query = new ExchangeRateHistoryQuery("USD", null!, 30);
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.ToCurrency);
    }

    [Test]
    public void Should_HaveError_When_ToCurrencyIsTooShort()
    {
        var query = new ExchangeRateHistoryQuery("USD", "EU", 30);
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.ToCurrency)
            .WithErrorMessage("To Currency must be a 3-character ISO currency code.");
    }

    [Test]
    public void Should_HaveError_When_ToCurrencyIsTooLong()
    {
        var query = new ExchangeRateHistoryQuery("USD", "EURR", 30);
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.ToCurrency)
            .WithErrorMessage("To Currency must be a 3-character ISO currency code.");
    }

    [Test]
    public void Should_NotHaveError_When_ToCurrencyIsValid()
    {
        var query = new ExchangeRateHistoryQuery("USD", "EUR", 30);
        var result = _validator.TestValidate(query);
        result.ShouldNotHaveValidationErrorFor(x => x.ToCurrency);
    }

    #endregion

    #region Days Validation Tests

    [TestCase(30)]
    [TestCase(90)]
    [TestCase(365)]
    public void Should_NotHaveError_When_DaysIsAllowed(int days)
    {
        var query = new ExchangeRateHistoryQuery("USD", "EUR", days);
        var result = _validator.TestValidate(query);
        result.ShouldNotHaveValidationErrorFor(x => x.Days);
    }

    [TestCase(0)]
    [TestCase(1)]
    [TestCase(7)]
    [TestCase(60)]
    [TestCase(100)]
    [TestCase(180)]
    [TestCase(366)]
    public void Should_HaveError_When_DaysIsNotAllowed(int days)
    {
        var query = new ExchangeRateHistoryQuery("USD", "EUR", days);
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.Days)
            .WithErrorMessage("Days must be 30, 90, or 365.");
    }

    #endregion

    #region Full Query Validation Tests

    [Test]
    public void Should_BeValid_When_AllFieldsAreValid()
    {
        var query = new ExchangeRateHistoryQuery("USD", "EUR", 30);
        var result = _validator.TestValidate(query);
        result.IsValid.Should().BeTrue();
    }

    [Test]
    public void Should_BeInvalid_When_MultipleFieldsAreInvalid()
    {
        var query = new ExchangeRateHistoryQuery("", "", 10);
        var result = _validator.TestValidate(query);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCountGreaterThan(1);
    }

    #endregion
}
