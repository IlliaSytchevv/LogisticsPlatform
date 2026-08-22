using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;

namespace LogisticsPlatform.Application.Abstractions.Messaging;

internal sealed class ValidationQueryHandlerDecorator<TQuery, TResponse>(
    IQueryHandler<TQuery, TResponse> inner,
    IEnumerable<IValidator<TQuery>> validators)
    : IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    public async Task<TResponse> Handle(TQuery query, CancellationToken cancellationToken)
    {
        if (validators.Any())
        {
            var context = new ValidationContext<TQuery>(query);
            var validationResults = await Task.WhenAll(
                validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            ValidationError[] errors = validationResults
                .Where(r => !r.IsValid)
                .SelectMany(r => r.AsErrors())
                .Distinct()
                .ToArray();

            if (errors.Length > 0)
                return ValidationResultFactory.CreateInvalidResult<TResponse>(errors);
        }

        return await inner.Handle(query, cancellationToken);
    }
}
