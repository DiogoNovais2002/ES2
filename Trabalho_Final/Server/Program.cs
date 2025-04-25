using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Server.Data;
using Server.Models;
using Server.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuração da Base de Dados
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

    ?.Replace("{DB_PASSWORD}", Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "default_password");
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
// Identidade (ASP.NET Identity)
// Substituir AddDefaultIdentity por AddIdentity para suportar roles
// Identidade (ASP.NET Identity)
// Usar tipos com chave int para User e Role
// Identidade (ASP.NET Identity) com chave int para User e Role
builder.Services.AddIdentity<User, IdentityRole<int>>(options => 
    options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Garantir que não há chamada anterior a AddDefaultIdentity ou AddRoles<IdentityRole>
// (remova quaisquer linhas como AddDefaultIdentity<User> ou AddRoles<IdentityRole>)            // LINHA ADICIONADA: habilita roles

// Suporte a APIs (Controllers) e Razor Pages
builder.Services.AddControllers();
builder.Services.AddRazorPages();
builder.Services.AddRazorPages();
builder.Services.AddControllers();
// Registar o EventService para injeção de dependência
builder.Services.AddScoped<Server.Services.EventService>();
// Registar o ActivityService para injeção de dependência
builder.Services.AddScoped<Server.Services.ActivityService>();

// Registrar serviços
builder.Services.AddScoped<UserService>();

// Configuração de CORS (restrita para o frontend)



// Corrs
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

// Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Event Management API",
        Version = "v1",
        Description = "API para Gestão de Eventos e Participantes - Trabalho Prático ES2"
        Description = "Trabalho Prático ES2"
    });
});

var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Event Management API v1");
        c.RoutePrefix = string.Empty; // Acessar Swagger na raiz (ex.: http://localhost:5000)
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
    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("An error occurred. Please try again later.");
        });
    });
    app.UseExceptionHandler("/Home/Error");
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

app.UseCors();

// AUTENTICAÇÃO E AUTORIZAÇÃO (LINHAS ADICIONADAS)
app.UseAuthentication();
app.UseAuthorization();

// Mapear endpoints
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.MapControllers();

// Aplicar migrações automaticamente
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

// Iniciar a aplicação
app.Run();