using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Server.Data;
using Server.Models;
using Server.Services;

var builder = WebApplication.CreateBuilder(args);

// Configuração da Base de Dados
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString)); // Usando PostgreSQL com Npgsql

// Identidade (ASP.NET Identity)
builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Suporte a APIs (Controllers)
builder.Services.AddControllers();

// Registrar serviços
builder.Services.AddScoped<UserService>();

// Configuração de CORS (restrita para o frontend)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5196", "https://localhost:7219")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // Necessário se usar autenticação com cookies
    });
});

// Configuração do Swagger para documentação da API
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Event Management API",
        Version = "v1",
        Description = "API para Gestão de Eventos e Participantes - Trabalho Prático ES2"
    });
});

var app = builder.Build();

// Configuração do Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Event Management API v1");
        c.RoutePrefix = string.Empty; // Acessar Swagger na raiz (ex.: http://localhost:5000)
    });
}
else
{
    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("An error occurred. Please try again later.");
        });
    });
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseCors("AllowFrontend"); // Aplicar política CORS

app.UseAuthentication();
app.UseAuthorization();

// Configuração de rotas
app.MapControllers(); // Mapear endpoints da API

// Aplicar migrações automaticamente
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

// Iniciar a aplicação
app.Run();