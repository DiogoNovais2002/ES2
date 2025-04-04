using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuração da Base de Dados
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?.Replace("{DB_PASSWORD}", Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "default_password");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Identidade
builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Adicionar suporte a APIs
builder.Services.AddControllers();

// Configurar CORS para permitir chamadas do Blazor WebAssembly
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor",
        policy => policy.WithOrigins("http://localhost:5196") // Porta do Blazor
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
});

// Adicionar Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "API",
        Version = "v1",
        Description = "TRABALHO PRATICO ES2"
    });
});

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Minha API v1");
        c.RoutePrefix = string.Empty;
    });
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Ativar CORS antes da autenticação
app.UseCors("AllowBlazor");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

app.Run();
