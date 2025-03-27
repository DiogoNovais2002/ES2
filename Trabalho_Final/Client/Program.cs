using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Client;
using Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Regista o HttpClient para chamadas à API
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5140/") }); // API!


// Regista o ApiService como injetável
builder.Services.AddScoped<ApiService>();

await builder.Build().RunAsync();