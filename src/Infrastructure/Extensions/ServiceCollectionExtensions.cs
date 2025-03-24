using System.Text;
using Application.Interfaces;
using Application.Interfaces.Email;
using Application.Interfaces.Repositories;
using Application.Interfaces.Token;
using Application.Services;
using Domain.Aggregates.UserAggregate;
using Infrastructure.Data;
using Infrastructure.Data.IdentityValidators;
using Infrastructure.Data.Repositories;
using Infrastructure.Services;
using Infrastructure.Services.Email;
using Infrastructure.Services.Options;
using Infrastructure.Services.Token;
using Infrastructure.Services.Token.Providers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using RazorLight;

namespace Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services
            .ConfigureEmail(config)
            .ConfigureIdentity()
            // Вызов ConfigureJwtAuthentication должен быть после ConfigureIdentity для возврата 401 статус-кода
            // вместо редиректа на страницу входа, вызванным дефолтными настройками cookie identity
            .ConfigureJwtAuthentication(config)
            .ConfigureContextAndDataSource(config)
            .AddUnitOfWork();
            
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
            .UseOptions(new RazorLightOptions
            {
                EnableDebugMode = true
            })
            .UseEmbeddedResourcesProject(typeof(EmailService).Assembly, "Infrastructure.RazorTemplates.EmailTemplates")
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
            .AddIdentity<User, IdentityRole<Guid>>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
                options.Tokens.ProviderMap.Add(
                    "EmailConfirmationTokenProvider",
                    new TokenProviderDescriptor(typeof(EmailConfirmationTokenProvider<User>)));
                options.Tokens.EmailConfirmationTokenProvider = "EmailConfirmationTokenProvider";
                    
                options.Tokens.ProviderMap.Add(
                    "PasswordResetToken",
                    new TokenProviderDescriptor(typeof(PasswordResetTokenProvider<User>)));
                options.Tokens.PasswordResetTokenProvider = "PasswordResetToken";
                    
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
            })
            .AddUserValidator<UserPhoneNumberValidator<User>>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddUserManager<AppUserManager>()
            .AddUserStore<AppUserStore>()
            .AddRoleStore<AppRoleStore>()
            .AddDefaultTokenProviders()
            .AddTokenProvider(TokenOptions.DefaultAuthenticatorProvider, typeof(DataProtectorTokenProvider<User>));

        services.AddTransient<EmailConfirmationTokenProvider<User>>();
        services.AddTransient<PasswordResetTokenProvider<User>>();

        return services;
    }

    private static IServiceCollection ConfigureContextAndDataSource(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("AppDbContext");

        var dataSourceBuilder = connectionString is null
            ? new NpgsqlDataSourceBuilder
            {
                ConnectionStringBuilder =
                {
                    ApplicationName = "Personal_cabinet#" + Environment.MachineName,
                    Host = Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? throw new ArgumentNullException("Host"),
                    Port = int.Parse(Environment.GetEnvironmentVariable("POSTGRES_PORT") ?? throw new ArgumentNullException("Port")),
                    Database = Environment.GetEnvironmentVariable("POSTGRES_DB") ?? throw new ArgumentNullException("Database"),
                    Username = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? throw new ArgumentNullException("Username"),
                    Password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? throw new ArgumentNullException("Password")
                }
            }
            : new NpgsqlDataSourceBuilder(connectionString);

        services.AddScoped<NpgsqlDataSource>(_ => dataSourceBuilder.Build());

        var serviceProvider = services.BuildServiceProvider();
        var dataSource = serviceProvider.GetRequiredService<NpgsqlDataSource>();

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(dataSource);
            options.EnableDetailedErrors();
            options.EnableSensitiveDataLogging();
        });

        return services;
    }

    private static IServiceCollection AddUnitOfWork(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}