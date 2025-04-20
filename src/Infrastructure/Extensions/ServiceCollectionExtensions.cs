using System.Text;
using Application.Interfaces;
using Application.Interfaces.Email;
using Application.Interfaces.Repositories;
using Application.Interfaces.Token;
using Application.Services;
using Domain.Aggregates.UserAggregate;
using Infrastructure.Data;
using Infrastructure.Data.Identity;
using Infrastructure.Data.Identity.TokenProviders;
using Infrastructure.Data.Identity.Validators;
using Infrastructure.Data.Outbox;
using Infrastructure.Data.Repositories;
using Infrastructure.Email;
using Infrastructure.Token;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using Quartz;
using Quartz.Logging;

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
            .RegisterQuartzBackgroundJobs()
            .AddUnitOfWork();
            
        return services;
    }
        
    private static IServiceCollection ConfigureJwtAuthentication(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<JwtOptions>(options =>
        {
            var issuer = config["JwtOptions:Issuer"] ?? GetUrlForCurrentEnvironment();
            var audience = config["JwtOptions:Audience"] ?? GetUrlForCurrentEnvironment();
            
            options.Issuer = issuer ?? throw new Exception("JWT issuer not set");
            options.Audience = audience ?? throw new Exception("JWT audience not set");
            options.AccessTokenExpiresInSeconds = config.GetValue<int>("JwtOptions:AccessTokenExpiresInSeconds");
            options.TokenType = config["JwtOptions:TokenType"] ?? throw new Exception("JWT token type not set");
            // TODO: Вынести в переменную среды/другое надёжное место
            options.Key = config["JwtOptions:Key"] ?? throw new Exception("Encryption key not set");
        });
        
        services.AddScoped<ITokenService, TokenService>();

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
            var jwtOptions = services.BuildServiceProvider().GetRequiredService<IOptions<JwtOptions>>().Value;
            
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateAudience = true,
                ValidAudience = jwtOptions.Audience,
                ValidateIssuer = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });
        
        return services;

        string? GetUrlForCurrentEnvironment()
        {
            var environment = services.BuildServiceProvider().GetRequiredService<IWebHostEnvironment>();
            var urls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS")?.Split(";") 
                       ?? throw new Exception("ASPNETCORE_URLS environment variable not set");

            if (environment.IsProduction())
                return urls.FirstOrDefault(u => u.StartsWith("https"));
            
            return urls.FirstOrDefault(u => u.StartsWith("https") || u.StartsWith("http"));
        }
    }
        
    private static IServiceCollection ConfigureEmail(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<EmailOptions>(config.GetSection("EmailOptions"));
        
        services
            .AddScoped<IEmailSender, MailkitSender>()
            .AddScoped<IEmailService, EmailService>()
            .AddRazorTemplating();

        return services;
    }

    private static IServiceCollection ConfigureIdentity(this IServiceCollection services)
    {
        var emailConfirmation = "EmailConfirmation";
        var passwordReset = "PasswordReset";
        
        services
            .AddIdentity<User, IdentityRole<Guid>>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                
                options.Tokens.EmailConfirmationTokenProvider = emailConfirmation;
                options.Tokens.PasswordResetTokenProvider = passwordReset;
            })
            .AddUserValidator<UserPhoneNumberValidator<User>>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddUserManager<AppUserManager>()
            .AddUserStore<AppUserStore>()
            .AddRoleStore<AppRoleStore>()
            .AddDefaultTokenProviders()
            .AddTokenProvider<RefreshTokenProvider<User>>(TokenProvider.RefreshProvider)
            .AddTokenProvider<EmailConfirmationTokenProvider<User>>(emailConfirmation)
            .AddTokenProvider<PasswordResetTokenProvider<User>>(passwordReset);

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
                    ApplicationName = "GasStationAPI#" + Environment.MachineName,
                    Host = Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? throw new Exception("Failed to configure database server host. POSTGRES_HOST environment variable not set"),
                    Port = int.Parse(Environment.GetEnvironmentVariable("POSTGRES_PORT") ?? throw new Exception("Failed to configure database server port. POSTGRES_PORT environment variable not set")),
                    Database = Environment.GetEnvironmentVariable("POSTGRES_DB") ?? throw new Exception("Failed to configure database name. POSTGRES_DB environment variable not set"),
                    Username = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? throw new Exception("Failed to configure database server user. POSTGRES_USER environment variable not set"),
                    Password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? throw new Exception("Failed to configure database server user password. POSTGRES_PASSWORD environment variable not set")
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
    
    private static IServiceCollection RegisterQuartzBackgroundJobs(this IServiceCollection services)
    {
        services.AddQuartz(configure =>
        {
            var outboxJobKey = new JobKey(nameof(OutboxBackgroundJob));
            configure
                .AddJob<OutboxBackgroundJob>(j => j.WithIdentity(outboxJobKey))
                .AddTrigger(trigger => trigger.ForJob(outboxJobKey)
                    .WithSimpleSchedule(scheduleBuilder => scheduleBuilder.WithIntervalInSeconds(3).RepeatForever()));
        });

        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

        return services;
    }
}