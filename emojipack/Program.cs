using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using emojipack.Backend;
using MudBlazor;
using MudBlazor.Services;

namespace emojipack
{
    public class Program
    {
        public static string ApiUrl = "https://api.emojipack.cf";
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddMudServices(x =>
            {
                x.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
            });
            builder.Services.AddScoped<AuthService>();
            builder.Services.AddScoped<QueryService>();
            builder.Services.AddBlazoredLocalStorage();
            await builder.Build().RunAsync();
        }
    }
}
