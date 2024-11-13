using System.Text;
using Application.Interfaces.Email;
using Application.Interfaces.Repositories;
using Application.Interfaces.Token;
using Application.Services;
using Domain.Models;
using Infrastructure.Data;
using Infrastructure.Data.Repositories;
using Infrastructure.Services;
using Infrastructure.Services.Email;
using Infrastructure.Services.Options;
using Infrastructure.Services.Token;
using Infrastructure.Services.Token.Providers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using RazorLight;

namespace Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services
                .ConfigureJwtAuthentication(config)
                .ConfigureEmail(config)
                .ConfigureIdentity()
                .AddDatabase(config)
                .AddRepositories();
            
            return services;
        }
        
        private static IServiceCollection ConfigureJwtAuthentication(this IServiceCollection services, IConfiguration config)
        {
            services
                .Configure<JwtOptions>(config.GetSection("JwtOptions"))
                .AddScoped<ITokenService, TokenService>();
            
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = 
                options.DefaultSignInScheme = 
                options.DefaultChallengeScheme = 
                options.DefaultScheme = 
                options.DefaultForbidScheme =
                options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateAudience = true,
                    ValidAudience = config["JwtOptions:Audience"],
                    ValidateIssuer = true,
                    ValidIssuer = config["JwtOptions:Issuer"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JwtOptions:Key"]!)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            return services;
        }
        
        private static IServiceCollection ConfigureEmail(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<EmailOptions>(config.GetSection("EmailOptions"));
            
            var razorEngine = new RazorLightEngineBuilder()
                .UseEmbeddedResourcesProject(typeof(EmailService).Assembly, config["EmailTemplateOptions:TemplatesDirectory"])
                .UseMemoryCachingProvider()
                .Build();
            
            services
                .AddSingleton<IRazorLightEngine>(razorEngine)
                .AddScoped<IEmailSender, MailkitSender>()
                .AddScoped<IEmailTemplate, RazorEmailTemplate>()
                .AddScoped<IEmailService, EmailService>();

            return services;
        }

        private static IServiceCollection ConfigureIdentity(this IServiceCollection services)
        {
            services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromDays(90);
            });
            
            services
                .AddIdentity<User, IdentityRole<long>>(options =>
                {
                    options.User.RequireUniqueEmail = true;
                    options.SignIn.RequireConfirmedEmail = true;
                    options.Tokens.ProviderMap.Add(
                        "EmailConfirmationTokenProvider",
                        new TokenProviderDescriptor(typeof(EmailConfirmationTokenProvider<User>)));
                    options.Tokens.EmailConfirmationTokenProvider = "EmailConfirmationTokenProvider";
                    
                    options.Password.RequiredLength = 8;
                })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddUserManager<AppUserManager>()
                .AddDefaultTokenProviders()
                .AddTokenProvider(TokenOptions.DefaultAuthenticatorProvider, typeof(DataProtectorTokenProvider<User>));

            services.AddTransient<EmailConfirmationTokenProvider<User>>();

            return services;
        }

        private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(config.GetConnectionString(nameof(AppDbContext))));

            return services;
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<ICardRepository, CardRepository>();
            
            return services;
        }
    }
}