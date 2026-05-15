using Insurance.Application.Common.Interfaces;
using Insurance.Domain.Interfaces;
using Insurance.Persistence.Context;
using Insurance.Persistence.Repositories;
using Insurance.Persistence.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Insurance.Persistence;

/// <summary>
/// Dependency injection extension for Persistence layer.
/// Registers DbContext, repositories, and Unit of Work pattern implementations.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds persistence layer services to the dependency injection container.
    /// Configures SQL Server database connection and repository implementations.
    /// </summary>
    /// <param name="services">Service collection to register services in</param>
    /// <param name="configuration">Application configuration containing connection strings</param>
    /// <returns>Service collection for fluent chaining</returns>
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        // Register DbContext with SQL Server
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not configured");

        services.AddDbContext<InsuranceDbContext>(options =>
        {
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(typeof(DependencyInjection).Assembly.FullName);
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
            });

            // Enable query logging in development
            if (configuration["Environment"] == "Development")
            {
                options.EnableSensitiveDataLogging();
                options.LogTo(Console.WriteLine);
            }
        });

        // Register Unit of Work - scoped for per-request lifetime
        services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();

        // Register generic and specific repositories
        services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();

        return services;
    }
}