using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configuração do HttpClient com a URL do Backend
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5140/") });


builder.Services.AddScoped<ApiService>();  // Se você tiver um serviço API, adicione aqui

await builder.Build().RunAsync();