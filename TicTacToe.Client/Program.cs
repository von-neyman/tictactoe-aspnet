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

        var http = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
        var config = await http.GetStringAsync("appsettings.json");
        var json = System.Text.Json.JsonDocument.Parse(config);
        var serverUrl = json.RootElement.GetProperty("ServerUrl").GetString();

        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(serverUrl!) });
        builder.Services.AddScoped<GameClient>();

        await builder.Build().RunAsync();
    }
}