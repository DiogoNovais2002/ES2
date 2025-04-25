using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuração da Base de Dados
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?.Replace("{DB_PASSWORD}", Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "default_password");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));  // Usando PostgreSQL com Npgsql

// Identidade (ASP.NET Identity)
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

// Corrs
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API",
        Version = "v1",
        Description = "Trabalho Prático ES2"
    });
});

var app = builder.Build();

// Pipeline
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

app.Run();