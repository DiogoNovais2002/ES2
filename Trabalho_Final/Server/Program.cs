using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuração da Base de Dados
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?.Replace("{DB_PASSWORD}", Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "default_password");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));  // Usando PostgreSQL com Npgsql

// Identidade (ASP.NET Identity)
builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();  // Associando Identity ao DbContext

// Suporte a APIs (Controllers)
builder.Services.AddControllers();

// Configuração do CORS para permitir chamadas do Blazor WebAssembly
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor",
        policy => policy.WithOrigins("http://localhost:5196")  // Porta do Blazor WebAssembly
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());  // Permitir credenciais (cookies)
});

// Configuração do Swagger para documentação da API
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "API",
        Version = "v1",
        Description = "Trabalho Prático ES2"  // Descrição da API
    });
});

var app = builder.Build();

// Configuração do Middleware
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();  // Habilitar a execução de migrações (em desenvolvimento)
    app.UseSwagger();  // Habilitar o Swagger para ver a documentação da API
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Minha API v1");
        c.RoutePrefix = string.Empty;  // Acessar Swagger na raiz
    });
}
else
{
    app.UseExceptionHandler("/Home/Error");  // Página de erro genérica
    app.UseHsts();  // Forçar HTTPS
}

app.UseHttpsRedirection();  // Redirecionar para HTTPS
app.UseStaticFiles();  // Servir arquivos estáticos (se houver)

app.UseRouting();  // Configuração do roteamento

// Ativar CORS antes da autenticação (permitir chamadas do frontend)
app.UseCors("AllowBlazor");

app.UseAuthentication();  // Habilitar autenticação com Identity
app.UseAuthorization();  // Habilitar autorização

// Configuração de rotas do Controller
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Mapear Razor Pages (se estiver usando)
app.MapRazorPages();
app.MapControllers();  // Mapear Controllers da API

// Aplicar migrações automaticamente durante a execução
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();  // Aplica a migração ao banco de dados
}

// Iniciar a aplicação
app.Run();
