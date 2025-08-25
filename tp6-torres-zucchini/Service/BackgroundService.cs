using Microsoft.EntityFrameworkCore;
using tp6_torres_zucchini.Data;

namespace tp6_torres_zucchini.Service
{
    public class LimpiezaConexionesService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public LimpiezaConexionesService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                // desconectar conexiones vencidas
                var limiteTiempo = DateTime.UtcNow.AddMinutes(-5);

                var conexionesExpiradas = await context.Conexiones
                    .Where(c => c.Activa && c.FechaHora <= limiteTiempo)
                    .ToListAsync(stoppingToken);

                conexionesExpiradas.ForEach(c => c.Activa = false);

                if (conexionesExpiradas.Count > 0)
                    await context.SaveChangesAsync(stoppingToken);

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}