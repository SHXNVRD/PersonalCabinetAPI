using API.Abstractions;
using API.Services;
using API.Services.Tcp;
using Application.Interfaces;
using Application.Users.DTOs;
using FluentValidation;
using Microsoft.OpenApi.Models;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

namespace API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration config)
        {
            services
                .ConfigureFluentValidation()
                .ConfigureSwagger()
                .AddScoped<ILinkService, LinkService>()
                .ConfigureTcp(config);

            return services;
        }
        
        private static IServiceCollection ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo {Title = "Personal Cabinet API", Version = "v1"});
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Description = "Enter the Bearer Authorization token",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "Jwt",
                    Scheme = "Bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
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

        private static IServiceCollection ConfigureTcp(this IServiceCollection services, IConfiguration config)
        {
            services
                .Configure<TcpOptions>(config.GetSection("TcpOptions"))
                .AddSingleton<ITcpServer, TcpServer>()
                .AddHostedService<TcpHostedService>();

            return services;
        }
    }
}