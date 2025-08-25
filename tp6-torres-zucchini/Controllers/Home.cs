using Microsoft.AspNetCore.Mvc;

namespace tp6_torres_zucchini.Controllers
{
    [ApiController]
    [Route("/")]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public IActionResult Bienvenida()
        {
            return Ok(new
            {
                Mensaje = "Bienvenido a la API de TP6 - Torres Zucchini",
                Descripcion = "Esta API permite conectar clientes, generar pedidos y consultar estados.",
                EndpointsDisponibles = new[]
                {
                    "POST /cliente/Ingresar",
                    "POST /cliente/Desconectar",
                    "GET /cliente/EstadoServidor/{conexionId}",
                    "GET /cliente/FechaServidor/{conexionId}",
                    "GET /cliente/ClientesConectados",
                    "GET /cliente/MensajesCliente/{conexionId}"
                }
            });
        }
    }
}
