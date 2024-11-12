using Application.Interfaces.Email;
using Infrastructure.Services;
using Infrastructure.Services.EmailServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RazorLight;

namespace Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureEmail(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<EmailOptions>(config.GetSection("EmailOptions"));
            
            var razorEngine = new RazorLightEngineBuilder()
                .UseEmbeddedResourcesProject(typeof(EmailService).Assembly, config["EmailTemplateOptions:TemplatesDirectory"])
                .UseMemoryCachingProvider()
                .Build();
            
            services.AddSingleton<IRazorLightEngine>(razorEngine);
            services.AddScoped<IEmailSender, MailkitSender>();
            services.AddScoped<IEmailTemplate, RazorEmailTemplate>();
            services.AddScoped<IEmailService, EmailService>();

            return services;
        }
    }
}