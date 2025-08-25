using Microsoft.AspNetCore.Mvc;
using tp6_torres_zucchini.Data;
using tp6_torres_zucchini.Service;
using System.Threading.Tasks;

namespace tp6_torres_zucchini.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClienteController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConexionService _conexionService;

        public ClienteController(ApplicationDbContext context, IConexionService conexionService)
        {
            _context = context;
            _conexionService = conexionService;
        }

        // DTO para Ingresar
        public class ClienteRequest
        {
            public int ClienteId { get; set; }
        }

        // DTO para Desconectar
        public class ConexionRequest
        {
            public int ConexionId { get; set; }
        }

        [HttpPost("Ingresar")]
        public async Task<IActionResult> Ingresar([FromBody] ClienteRequest request)
        {
            var conexionId = await _conexionService.ConectarAsync(request.ClienteId);

            if (conexionId.StartsWith("ERROR"))
            {
                return BadRequest(conexionId); // Ej: "ERROR Ya existe una conexión activa para este cliente"
            }

            return Ok(new
            {
                ClienteId = request.ClienteId,
                ConexionId = conexionId
            });
        }

        [HttpPost("Desconectar")]
        public async Task<IActionResult> Desconectar([FromBody] ConexionRequest request)
        {
            var result = await _conexionService.DesconectarAsync(request.ConexionId);

            if (result.StartsWith("ERROR"))
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("EstadoServidor/{conexionId}")]
        public async Task<IActionResult> EstadoServidor(int conexionId)
        {
            var estado = await _conexionService.ObtenerEstadoServidorAsync(conexionId);
            return Ok(new
            {
                ConexionId = conexionId,
                EstadoServidor = estado
            });
        }

        [HttpGet("FechaServidor/{conexionId}")]
        public async Task<IActionResult> FechaServidor(int conexionId)
        {
            var fecha = await _conexionService.ObtenerFechaServidorAsync(conexionId);
            return Ok(new
            {
                ConexionId = conexionId,
                FechaServidor = fecha
            });
        }
    }
}
