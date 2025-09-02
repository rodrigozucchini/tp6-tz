using Microsoft.AspNetCore.Mvc;
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

        public AdminConexion(IMonitorService monitorService, ILogger<AdminConexion> logger)
        {
            _monitorService = monitorService;
            _logger = logger;
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
    }
}
