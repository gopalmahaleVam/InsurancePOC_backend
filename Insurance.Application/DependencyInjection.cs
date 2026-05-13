using FluentValidation;
using Insurance.Application.Common;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Insurance.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // ✅ MediatR
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            // ✅ FluentValidation (NOW WORKS ✅)
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            // ✅ Pipeline
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            //services.AddAutoMapper(typeof(DependencyInjection).Assembly);
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
