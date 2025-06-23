using System.Globalization;
using ApiClient;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Web.Services;
using Web.Services.Authentication;
using Web.Services.Users;

namespace Web;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        var config = builder.Configuration;
        var services = builder.Services;
        
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");
        
        services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
        services.AddBlazoredLocalStorage();
        services.AddAuthorizationCore();
        services.AddCascadingAuthenticationState();
        
        services.AddScoped<AuthenticationStateProvider, JwtAuthStateProvider>();
        services.AddScoped<AuthenticationService>();
        services.Configure<RefreshTokenOptions>(config.GetSection(nameof(RefreshTokenOptions)));
        services.AddScoped<RefreshTokenService>();
        services.AddScoped<ThemeService>();
        
        services.AddScoped<RefreshTokenHandler>();
        services.AddScoped<UnauthorizedHandler>();

        services.AddHttpClient<AuthenticationService>(c =>
        {
            c.BaseAddress = new Uri("https://localhost:8082/api/");
        });
        
        services.AddHttpClient<GasStationClient>(c =>
        {
            c.BaseAddress = new Uri("https://localhost:8082");
        }).AddHttpMessageHandler<UnauthorizedHandler>()
            .AddHttpMessageHandler<RefreshTokenHandler>();
        
        services.AddScoped<UserService>();
            
        services.AddMudServices();
        
        var culture = new CultureInfo("ru-RU");
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
        
        await builder.Build().RunAsync();
    }
}