using Application.Behaviors;
using Application.Users;
using Application.Users.DTOs;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

namespace Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services
                .ConfigureMediatR()
                .ConfigureFluentValidation();

            return services;
        }
        
        private static IServiceCollection ConfigureMediatR(this IServiceCollection services)
        {
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssemblyContaining<AuthResponse>();
                config.AddOpenBehavior(typeof(RequestLoggningBehavior<,>));
            });
            
            return services;
        }

        private static IServiceCollection ConfigureFluentValidation(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<AuthResponse>();
            services.AddFluentValidationAutoValidation(config =>
            {
                config.DisableBuiltInModelValidation = true;
            });
            
            return services;
        }
    }
}