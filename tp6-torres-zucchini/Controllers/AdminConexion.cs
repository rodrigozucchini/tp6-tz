using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
using System.ComponentModel;
using tp6_torres_zucchini.Data;
using tp6_torres_zucchini.Data.Models;
using tp6_torres_zucchini.Service;

namespace tp6_torres_zucchini.Controllers
{
    [ApiController]
    [Route("admin/[controller]")]
    [ApiExplorerSettings(GroupName = "PanelAdmin")]
    public class AdminConexion : ControllerBase
    {
        private readonly IMonitorService _monitorService;
        private readonly ILogger<AdminConexion> _logger;
        private readonly ApplicationDbContext _context;

        public AdminConexion(IMonitorService monitorService, ILogger<AdminConexion> logger, ApplicationDbContext context)
        {
            _monitorService = monitorService;
            _logger = logger;
            _context = context;
        }

        [HttpGet("ConexionesActivas")]
        public async Task<IActionResult> GetConexiones()
        {
            var conexiones = await _monitorService.ObtenerConexionesActivasAsync();

            // Log en terminal usando ILogger (para las peticiones HTTP)
            _logger.LogInformation("HTTP REQUEST: Se registraron {Count} conexiones activas", conexiones.Count);
            foreach (var conexion in conexiones)
            {
                _logger.LogInformation("HTTP REQUEST Conexion: ClienteId={ClienteId}, FechaHora={FechaHora}",
                    conexion.ClienteId, conexion.FechaHora);
            }

            return Ok(conexiones);
        }

        [HttpGet("Comandos")]
        public async Task<IActionResult> UltimosComandos()
        {
            var comandos = await _monitorService.ObtenerUltimosComandosAsync();

            // Log en la terminal (para las peticiones HTTP)
            _logger.LogInformation("HTTP REQUEST: Se ejecutaron {Count} comandos", comandos.Count);
            foreach (var comando in comandos)
            {
                _logger.LogInformation("HTTP REQUEST Log: Comando={Comando}, ConexionId={ConexionId}, RespuestaComando={RespuestaComando}, FechaHora={FechaHora}",
                   comando.Comando, comando.ConexionId, comando.RespuestaComando, comando.FechaHora);
            }

            return Ok(comandos);
        }

        [HttpGet("DescargarLogs")]
        public async Task<IActionResult> DescargarLogs([FromQuery, DefaultValue("2025-09-01T00:00:00")] DateTime fechaInicio,[FromQuery, DefaultValue("2025-09-03T23:59:59")] DateTime fechaFin)
        {
            // Traer los logs entre las fechas
            var logs = await _context.Set<LogPeticion>()
                .Where(l => l.FechaHora >= fechaInicio && l.FechaHora <= fechaFin)
                .OrderBy(l => l.FechaHora)
                .ToListAsync();

            if (logs.Count == 0)
                return NotFound("No hay logs en el rango de fechas especificado.");

            // Generar el contenido del archivo
            var contenido = new System.Text.StringBuilder();
            foreach (var log in logs)
            {
                contenido.AppendLine($"ID: {log.Id}");
                contenido.AppendLine($"FechaHora: {log.FechaHora}");
                contenido.AppendLine($"Comando: {log.Comando}");
                contenido.AppendLine($"Respuesta: {log.RespuestaComando}");
                contenido.AppendLine(new string('-', 50));
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(contenido.ToString());
            var nombreArchivo = $"Logs_{fechaInicio:yyyyMMdd}_{fechaFin:yyyyMMdd}.txt";

            return File(bytes, "text/plain", nombreArchivo);
        }
    }
}
