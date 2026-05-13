using Microsoft.Extensions.DependencyInjection;

namespace Insurance.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            // JWT, Email, Logging later
            return services;
        }
    }
}