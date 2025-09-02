using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using tp6_torres_zucchini.Data;
using tp6_torres_zucchini.Service;

var builder = WebApplication.CreateBuilder(args);

// ------------------ Servicios ------------------
// Razor Pages (para tus p�ginas Admin y Home)
builder.Services.AddRazorPages();

// Registro del filtro como servicio (NUEVO)
builder.Services.AddScoped<LogActionFilter>();

// Controllers para la API con filtro
builder.Services.AddControllers(options =>
{
    // Registro del filtro de acci�n
    options.Filters.Add<LogActionFilter>();
});

// Swagger (documentaci�n autom�tica)
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

// Inyecci�n de dependencias de tus servicios
builder.Services.AddScoped<IConexionService, ConexionService>();
builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddScoped<IMonitorService, MonitorService>(); // ?? NUEVO: Servicio de monitoreo

// BackgroundService para limpieza autom�tica
builder.Services.AddHostedService<LimpiezaConexionesService>();

// Registro del Hub de SignalR
builder.Services.AddSignalR();

var app = builder.Build();

// Mapear Hub ANTES del middleware (mejor pr�ctica)
app.MapHub<MonitorHub>("/monitorHub");

// ------------------ Middleware ------------------
// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // ?? NUEVO: Para ver errores detallados
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