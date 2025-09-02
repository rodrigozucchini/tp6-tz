using Microsoft.EntityFrameworkCore;
using tp6_torres_zucchini.Data;
using tp6_torres_zucchini.Data.Models;

namespace tp6_torres_zucchini.Service
{
    public interface IMonitorService
    {
        Task<List<Conexion>> ObtenerConexionesActivasAsync();
        Task<List<LogPeticion>> ObtenerUltimosComandosAsync();
        Task LogConexionesActivasAsync();
        Task LogUltimosComandosAsync();
    }

    public class MonitorService : IMonitorService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MonitorService> _logger;

        public MonitorService(ApplicationDbContext context, ILogger<MonitorService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Método que devuelve datos (para el controlador)
        public async Task<List<Conexion>> ObtenerConexionesActivasAsync()
        {
            return await _context.Conexiones
                .Where(c => c.Activa)
                .OrderByDescending(c => c.FechaHora)
                .Take(10)
                .ToListAsync();
        }

        // Método que devuelve datos (para el controlador)
        public async Task<List<LogPeticion>> ObtenerUltimosComandosAsync()
        {
            return await _context.LogPeticiones
                .OrderByDescending(c => c.FechaHora)
                .Take(10)
                .ToListAsync();
        }

        // Método que solo hace logging (para el filtro)
        public async Task LogConexionesActivasAsync()
        {
            try
            {
                var conexiones = await ObtenerConexionesActivasAsync();

                _logger.LogInformation("🔄 AUTO-EJECUTADO: Se registraron {Count} conexiones activas", conexiones.Count);
                foreach (var conexion in conexiones)
                {
                    _logger.LogInformation("🔄 AUTO-EJECUTADO Conexion: ClienteId={ClienteId}, FechaHora={FechaHora}",
                        conexion.ClienteId, conexion.FechaHora);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en LogConexionesActivasAsync");
            }
        }

        // Método que solo hace logging (para el filtro)
        public async Task LogUltimosComandosAsync()
        {
            try
            {
                var comandos = await ObtenerUltimosComandosAsync();

                _logger.LogInformation("🔄 AUTO-EJECUTADO: Se ejecutaron {Count} comandos", comandos.Count);
                foreach (var comando in comandos)
                {
                    // Truncar para evitar el error de base de datos
                    var respuestaCorta = comando.RespuestaComando?.Length > 100
                        ? comando.RespuestaComando.Substring(0, 100) + "..."
                        : comando.RespuestaComando;

                    _logger.LogInformation("🔄 AUTO-EJECUTADO Log: Comando={Comando}, ConexionId={ConexionId}, RespuestaComando={RespuestaComando}, FechaHora={FechaHora}",
                        comando.Comando, comando.ConexionId, respuestaCorta, comando.FechaHora);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en LogUltimosComandosAsync");
            }
        }
    }
}