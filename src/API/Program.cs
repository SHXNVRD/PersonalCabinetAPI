using API.Extensions;
using API.Middlewares;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Options;
using Application.Services;
using Domain.Models;
using FluentValidation;
using Infrastructure.Data;
using Infrastructure.Data.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Application.Behaviors;
using Application.Interfaces.Services;
using Application.Users.DTOs;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Host.ConfigureSerilog();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.Configure<JwtOptions>(config.GetSection("JwtOptions"));
builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromDays(90);
});

builder.Services
    .AddIdentity<User, IdentityRole<long>>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.Password.RequiredLength = 8;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddUserManager<AppUserManager>()
    .AddDefaultTokenProviders()
    .AddTokenProvider(TokenOptions.DefaultAuthenticatorProvider, typeof(DataProtectorTokenProvider<User>));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString(nameof(AppDbContext))));

builder.Services.AddAuthorization();
builder.Services.AddAuthentication();

builder.Services.ConfigureJwtAuthentication(config);
builder.Services.ConfigureSwagger();
builder.Services.AddHttpContextAccessor();

builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<AuthResponse>();
    cfg.AddOpenBehavior(typeof(RequestLoggningPipelineBehavior<,>));
});

builder.Services.AddValidatorsFromAssemblyContaining<AuthResponse>();
builder.Services.AddFluentValidationAutoValidation(cfg =>
{
    cfg.DisableBuiltInModelValidation = true;
});
builder.Services.AddScoped<ICardRepository, CardRepository>();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();
ResultExtensions.Configure(app.Services.GetRequiredService<IHttpContextAccessor>());
app.UseSerilogRequestLogging();
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
