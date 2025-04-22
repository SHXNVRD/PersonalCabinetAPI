using ApiClient.Extensions;
using Blazored.LocalStorage;
using Blazorise;
using Blazorise.Icons.FontAwesome;
using Blazorise.Tailwind;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Web;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        var services = builder.Services;
        
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");
        
        services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
        services.AddBlazoredLocalStorage();
        services.AddAuthorizationCore();
        services.AddCascadingAuthenticationState();
        
        services
            .AddBlazorise()
            .AddTailwindProviders()
            .AddFontAwesomeIcons()
            .AddApiClients();
        
        await builder.Build().RunAsync();
    }
}