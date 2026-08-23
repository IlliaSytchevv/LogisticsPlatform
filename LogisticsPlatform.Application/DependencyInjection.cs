using System.Reflection;
using FluentValidation;
using LogisticsPlatform.Application.Abstractions.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace LogisticsPlatform.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        Assembly assembly = typeof(DependencyInjection).Assembly;

        services.AddValidatorsFromAssembly(assembly);
        services.AddScoped<IDispatcher, Dispatcher>();

        RegisterHandlers(
            services,
            assembly,
            typeof(ICommandHandler<,>),
            typeof(ValidationCommandHandlerDecorator<,>));

        RegisterHandlers(
            services,
            assembly,
            typeof(IQueryHandler<,>),
            typeof(ValidationQueryHandlerDecorator<,>));

        return services;
    }

    private static void RegisterHandlers(
        IServiceCollection services,
        Assembly assembly,
        Type handlerInterfaceDefinition,
        Type decoratorDefinition)
    {
        IEnumerable<(Type HandlerType, Type InterfaceType)> handlerRegistrations = assembly
            .GetTypes()
            .Where(type => type is { IsAbstract: false, IsInterface: false })
            .SelectMany(type => type
                .GetInterfaces()
                .Where(@interface =>
                    @interface.IsGenericType &&
                    @interface.GetGenericTypeDefinition() == handlerInterfaceDefinition)
                .Select(@interface => (HandlerType: type, InterfaceType: @interface)));

        foreach ((Type handlerType, Type interfaceType) in handlerRegistrations)
        {
            Type[] genericArguments = interfaceType.GetGenericArguments();
            Type decoratorType = decoratorDefinition.MakeGenericType(genericArguments);

            services.AddScoped(handlerType);
            services.AddScoped(interfaceType, serviceProvider =>
            {
                object inner = serviceProvider.GetRequiredService(handlerType);
                return ActivatorUtilities.CreateInstance(serviceProvider, decoratorType, inner);
            });
        }
    }
}
