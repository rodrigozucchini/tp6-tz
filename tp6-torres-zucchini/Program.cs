using Microsoft.EntityFrameworkCore;
using tp6_torres_zucchini.Data;
using tp6_torres_zucchini.Service;

var builder = WebApplication.CreateBuilder(args);

// ------------------ Servicios ------------------

// Razor Pages (para tus p�ginas Admin y Home)
builder.Services.AddRazorPages();

// Controllers para la API
builder.Services.AddControllers();

// Swagger (documentaci�n autom�tica)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DbContext con SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Inyecci�n de dependencias de tus servicios
builder.Services.AddScoped<IConexionService, ConexionService>();
builder.Services.AddScoped<ILogService, LogService>();

// BackgroundService para limpieza autom�tica
builder.Services.AddHostedService<LimpiezaConexionesService>();

var app = builder.Build();

// ------------------ Middleware ------------------

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
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