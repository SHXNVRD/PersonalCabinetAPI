using API.Extensions;
using API.Middlewares;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Services;
using Domain.Models;
using FluentValidation;
using Infrastructure.Data;
using Infrastructure.Data.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Application.Behaviors;
using Application.Extensions;
using Application.Interfaces.Token;
using Application.Users.DTOs;
using Infrastructure.Extensions;
using Infrastructure.Services.Options;
using Infrastructure.Services.Token;
using Microsoft.AspNetCore.HttpOverrides;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
IConfiguration config = builder.Configuration;
IServiceCollection services = builder.Services;

builder.Host.ConfigureSerilog();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();
builder.Services.ConfigureSwagger();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

services
    .AddInfrastructure(config)
    .AddApplication();

services
    .AddAuthorization()
    .AddAuthentication();

builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddHttpContextAccessor();

var app = builder.Build();
ResultExtensions.Configure(app.Services.GetRequiredService<IHttpContextAccessor>());
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "{RemoteIpAddress} {RequestHost} {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
    options.EnrichDiagnosticContext = (context, httpContext) =>
    {
        context.Set("RemoteIpAddress", httpContext.Connection.RemoteIpAddress);
        context.Set("RequestHost", httpContext.Request.Host);
    };
});

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