using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Insurance.Persistence.Context;
using Insurance.Persistence.Repositories;
using Insurance.Domain.Interfaces;

namespace Insurance.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration config)
{
    services.AddDbContext<InsuranceDbContext>(options =>
        options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

    services.AddScoped<IUserRepository, UserRepository>();

    return services;
}

    }
}