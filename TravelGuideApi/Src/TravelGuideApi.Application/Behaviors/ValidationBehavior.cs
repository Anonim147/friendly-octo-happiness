using FluentValidation;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation.Results;

namespace TravelGuideApi.Application.Behaviors;

/// <summary>
/// See https://github.com/jbogard/MediatR/wiki/Behaviors
/// </summary>
/// <typeparam name="TRequest">The request object passed in through IMediator.Send</typeparam>
/// <typeparam name="TResponse">Handler's response</typeparam>
public class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private IEnumerable<IValidator<TRequest>> _validators = validators;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            ValidationContext<TRequest> context = new(request);

            ValidationResult[] validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            ValidationFailure[] failures = validationResults.SelectMany(r => r.Errors)
                .Where(f => f != null).ToArray();

            if (failures.Length != 0)
            {
                throw new ValidationException(failures);
            }
        }

        return await next();
    }
}
