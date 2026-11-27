using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TicTacToe.Client.Service;

namespace TicTacToe.Client;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        var serverUrl = builder.Configuration["ServerUrl"] ?? builder.HostEnvironment.BaseAddress;
        builder.Services.AddScoped<AuthService>();
        builder.Services.AddScoped<ScoreService>();
        builder.Services.AddHttpClient<GameClient>(client => client.BaseAddress = new Uri(serverUrl));
        builder.Services.AddHttpClient<AuthClient>(client => client.BaseAddress = new Uri(serverUrl));

        await builder.Build().RunAsync();
    }
}