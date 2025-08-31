using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using tp6_torres_zucchini.Data;
using tp6_torres_zucchini.Service;

var builder = WebApplication.CreateBuilder(args);

// ------------------ Servicios ------------------

// Razor Pages (para tus páginas Admin y Home)
builder.Services.AddRazorPages();

// Controllers para la API
builder.Services.AddControllers();

// Swagger (documentación automática)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Panel Cliente", Version = "v1" });
    c.SwaggerDoc("PanelAdmin", new OpenApiInfo { Title = "Panel Admin", Version = "v1" });

    c.DocInclusionPredicate((docName, apiDesc) =>
    {
        var groupName = apiDesc.GroupName ?? "v1";
        return groupName == docName;
    });
});

// DbContext con SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Inyección de dependencias de tus servicios
builder.Services.AddScoped<IConexionService, ConexionService>();
builder.Services.AddScoped<ILogService, LogService>();

// BackgroundService para limpieza automática
builder.Services.AddHostedService<LimpiezaConexionesService>();

var app = builder.Build();

// ------------------ Middleware ------------------

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API General v1");
        c.SwaggerEndpoint("/swagger/PanelAdmin/swagger.json", "Panel Admin v1");
        c.RoutePrefix = "swagger"; // default /swagger
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Mapear API
app.MapControllers();

// Mapear Razor Pages
app.MapRazorPages();

app.Run();