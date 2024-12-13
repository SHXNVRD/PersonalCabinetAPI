using Application.Behaviors;
using Application.Users;
using Application.Users.DTOs;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services
                .ConfigureMediatR();

            return services;
        }
        
        private static IServiceCollection ConfigureMediatR(this IServiceCollection services)
        {
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssemblyContaining<AuthResponse>();
                config.AddOpenBehavior(typeof(RequestLoggingBehavior<,>));
            });
            
            return services;
        }
    }
}