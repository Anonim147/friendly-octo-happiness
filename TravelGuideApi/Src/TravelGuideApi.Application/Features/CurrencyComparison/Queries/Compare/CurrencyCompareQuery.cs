using MediatR;
using TravelGuideApi.Domain.Models;

namespace TravelGuideApi.Application.Features.CurrencyComparison.Queries.Compare;

/// <summary>
/// Query to compare currencies between two countries.
/// </summary>
/// <param name="HomeCountryCode">ISO 3166-1 alpha-2 code of the home country.</param>
/// <param name="DestinationCountryCode">ISO 3166-1 alpha-2 code of the destination country.</param>
public record CurrencyCompareQuery(
    string HomeCountryCode,
    string DestinationCountryCode)
    : IRequest<CurrencyComparisonResult>;
