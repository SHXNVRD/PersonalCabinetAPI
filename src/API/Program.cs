using System.Text;
using API.Extensions;
using API.HostedServices;
using API.Middlewares;
using Serilog;
using Application.Extensions;
using Infrastructure.Extensions;
using Tcp.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
IConfiguration config = builder.Configuration;
IServiceCollection services = builder.Services;

builder.Host.ConfigureSerilog();
services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddProblemDetails();

services.AddExceptionHandler<GlobalExceptionHandler>();

services
    .AddInfrastructure(config)
    .AddApplication()
    .AddApi(config)
    .AddTcp(config).AddHostedService<TcpHostedService>();

services
    .AddAuthorization()
    .AddAuthentication();

services.AddRouting(options => options.LowercaseUrls = true);

services.AddHttpContextAccessor();

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

var app = builder.Build();
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "Remote ip: {RemoteIpAddress} {RequestHost} {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
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
    app.ApplyMigrations();
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();