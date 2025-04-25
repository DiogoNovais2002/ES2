using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Client;
using Client.Services;
using System;
using System.Net.Http;
using System.Threading.Tasks;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// HttpClient para sua API
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5140/") });

// seu ApiService existente
builder.Services.AddScoped(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var apiBaseUrl = configuration["ApiBaseUrl"] ?? "http://localhost:5000/";
    return new HttpClient { BaseAddress = new Uri(apiBaseUrl) };
});

// Regista o ApiService como injetável
builder.Services.AddScoped<ApiService>();

// registra o UserService
builder.Services.AddScoped<UserService>();

await builder.Build().RunAsync();