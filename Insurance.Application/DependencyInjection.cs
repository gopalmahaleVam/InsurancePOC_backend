using FluentValidation;
using Insurance.Application.Common;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Insurance.Application;

/// <summary>
/// Dependency injection extension for Application layer.
/// Registers MediatR handlers, validators, and AutoMapper profiles.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds application layer services to the dependency injection container.
    /// Configures CQRS via MediatR, validation via FluentValidation, and mapping via AutoMapper.
    /// </summary>
    /// <param name="services">Service collection to register services in</param>
    /// <returns>Service collection for fluent chaining</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register MediatR for CQRS pattern
        // Scans assembly for command/query handlers automatically
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // Register FluentValidation validators
        // Scans assembly for validator implementations
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // Register validation pipeline behavior
        // Executes before each MediatR handler to validate requests
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        // Register AutoMapper profiles for DTO mappings
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        return services;
    }
}
