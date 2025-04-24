using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Server.Data;
using Server.Models;
using Server.Services;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Configuração da Base de Dados
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?.Replace("{DB_PASSWORD}", Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "default_password");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));  // Usando PostgreSQL com Npgsql

// Configuração do ASP.NET Identity
builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();  // Associando Identity ao DbContext

// Suporte a APIs (Controllers)
builder.Services.AddControllers();

// Suporte a CORS (Cross-Origin Resource Sharing)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Configuração do Swagger para documentação da API
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "API",
        Version = "v1",
        Description = "Trabalho Prático ES2"
    });
});

// Adicionando serviço de JWT
builder.Services.AddScoped<ITokenService, TokenService>();

// Adicionar configuração de JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddAuthorization();

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

app.UseCors(); // Ativar CORS

app.UseAuthentication();  // Habilitar autenticação com Identity e JWT
app.UseAuthorization();  // Habilitar autorização

// Configuração de rotas do Controller
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

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
