using System.Reflection;
using Ardalis.Result;
using FluentValidation;
using FluentValidation.Results;

namespace LogisticsPlatform.Application.Abstractions.Messaging;

internal static class ValidationResultFactory
{
    public static TResponse CreateInvalidResult<TResponse>(ValidationError[] errors)
    {
        Type responseType = typeof(TResponse);

        if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            Type resultValueType = responseType.GetGenericArguments()[0];
            MethodInfo? invalidMethod = typeof(Result<>)
                .MakeGenericType(resultValueType)
                .GetMethod(nameof(Result<int>.Invalid), [typeof(ValidationError[])]);

            if (invalidMethod is null)
                throw new InvalidOperationException($"Cannot create Invalid result for {responseType.Name}.");

            return (TResponse)invalidMethod.Invoke(null, [errors])!;
        }

        if (responseType == typeof(Result))
        {
            MethodInfo? invalidMethod = typeof(Result)
                .GetMethod(nameof(Result.Invalid), [typeof(ValidationError[])]);

            if (invalidMethod is null)
                throw new InvalidOperationException("Cannot create Invalid Result.");

            return (TResponse)invalidMethod.Invoke(null, [errors])!;
        }

        throw new ValidationException(errors.Select(e => new ValidationFailure(e.Identifier, e.ErrorMessage)));
    }
}
