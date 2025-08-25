using Microsoft.AspNetCore.Mvc;
using tp6_torres_zucchini.Service;

namespace tp6_torres_zucchini.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PedidoController : ControllerBase
    {
        private readonly IConexionService _conexionService;

        public PedidoController(IConexionService conexionService)
        {
            _conexionService = conexionService;
        }

        // 🔹 Verifica si la conexión está activa antes de seguir
        private async Task<bool> ConexionActiva(int conexionId)
        {
            var estado = await _conexionService.ObtenerEstadoServidorAsync(conexionId);
            return estado == "ACTIVO";
        }

        public class GenerarPedidoRequest
        {
            public int ConexionId { get; set; }
        }

        [HttpPost("Generar")]
        public async Task<IActionResult> Generar([FromBody] GenerarPedidoRequest request)
        {
            if (request.ConexionId <= 0)
                return BadRequest("Conexión inválida.");

            if (!await ConexionActiva(request.ConexionId))
                return BadRequest("Conexión no activa o inexistente.");

            var nuevoPedidoId = await _conexionService.GenerarPedidoAsync(request.ConexionId);
            return Ok(new { PedidoId = nuevoPedidoId });
        }

        [HttpGet("Consultar/{conexionId}/{pedidoId}")]
        public async Task<IActionResult> Consultar(int conexionId, int pedidoId)
        {
            if (conexionId <= 0 || pedidoId <= 0)
                return BadRequest("Datos inválidos.");

            if (!await ConexionActiva(conexionId))
                return BadRequest("Conexión no activa o inexistente.");

            var estado = await _conexionService.ConsultarEstadoPedidoAsync(conexionId, pedidoId);
            return Ok(new { Estado = estado });
        }

        [HttpPost("CambiarEstado")]
        public async Task<IActionResult> CambiarEstado(int conexionId, int pedidoId, string nuevoEstado)
        {
            if (conexionId <= 0 || pedidoId <= 0 || string.IsNullOrWhiteSpace(nuevoEstado))
                return BadRequest("Datos inválidos.");

            if (!await ConexionActiva(conexionId))
                return BadRequest("Conexión no activa o inexistente.");

            var result = await _conexionService.CambiarEstadoPedidoAsync(conexionId, pedidoId, nuevoEstado);
            if (result.StartsWith("ERROR"))
                return BadRequest(result);

            return Ok(result);
        }
    }
}
