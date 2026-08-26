using Ardalis.Result;
using LogisticsPlatform.Application.Abstractions.Messaging;

namespace LogisticsPlatform.Application.UseCases.Payments.HandleWebhook;

public sealed record HandleStripeWebhookCommand(string JsonBody, string StripeSignatureHeader)
    : ICommand<Result>;
