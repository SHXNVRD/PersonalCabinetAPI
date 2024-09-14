using API.Extensions;
using API.Middlewares;
using Application.DTOs;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Options;
using Application.Services;
using Application.Users.Commands.Registration;
using Domain.Models;
using FluentValidation;
using Infrastructure.Data;
using Infrastructure.Data.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
    .AddDefaultTokenProviders()
    .AddTokenProvider(TokenOptions.DefaultAuthenticatorProvider, typeof(DataProtectorTokenProvider<User>));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString(nameof(AppDbContext))));

builder.Services.AddAuthorization();
builder.Services.AddAuthentication();

builder.ConfigureJwtAuthentication();
builder.ConfigureSwagger();

builder.Services.AddProblemDetails();
builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssemblyContaining<AuthResponse>();
});
builder.Services.AddValidatorsFromAssemblyContaining<AuthResponse>();
builder.Services.AddFluentValidationAutoValidation(config =>
{
    config.DisableBuiltInModelValidation = true;
});
builder.Services.AddScoped<ICardRepository, CardRepository>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

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
