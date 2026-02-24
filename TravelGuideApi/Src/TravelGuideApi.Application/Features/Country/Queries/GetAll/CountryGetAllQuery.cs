using System.Collections.Generic;
using MediatR;
using TravelGuideApi.Domain.Entities;

namespace TravelGuideApi.Application.Features.Country.Queries.GetAll;

/// <summary>
/// Query to get all countries with their currency information.
/// </summary>
public record CountryGetAllQuery : IRequest<IEnumerable<CountryEntity>>;
