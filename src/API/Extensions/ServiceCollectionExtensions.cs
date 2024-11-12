using System.Collections.Immutable;
using System.Text;
using Application.Options;
using Application.Services;
using Domain.Models;
using Infrastructure.Data;
using Infrastructure.Tokens.Providers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtOptions = configuration
                .GetSection(nameof(JwtOptions))
                .Get<JwtOptions>() ?? throw new Exception();

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
        }

        public static IServiceCollection ConfigureSwagger(this IServiceCollection services)
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

        public static IServiceCollection ConfigureIdentity(this IServiceCollection services)
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
    }
}