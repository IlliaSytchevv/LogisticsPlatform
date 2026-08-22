using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;

namespace LogisticsPlatform.Application.Abstractions.Messaging;

internal sealed class ValidationCommandHandlerDecorator<TCommand, TResponse>(
    ICommandHandler<TCommand, TResponse> inner,
    IEnumerable<IValidator<TCommand>> validators)
    : ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    public async Task<TResponse> Handle(TCommand command, CancellationToken cancellationToken)
    {
        if (validators.Any())
        {
            var context = new ValidationContext<TCommand>(command);
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

        return await inner.Handle(command, cancellationToken);
    }
}
