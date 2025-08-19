using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using tp6_torres_zucchini.Data;
using tp6_torres_zucchini.Data.Models;
using tp6_torres_zucchini.Service;

namespace tp6_torres_zucchini.Pages.User
{
    public class PedidosModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IConexionService _conexionService;

        public PedidosModel(ApplicationDbContext context, IConexionService conexionService)
        {
            _context = context;
            _conexionService = conexionService;
        }

        [BindProperty]
        public int idCliente { get; set; }

        [BindProperty]
        public int ConexionId { get; set; }

        [BindProperty]
        public int PedidoId { get; set; }

        [BindProperty]
        public string NuevoEstado { get; set; }

        public string EstadoPedido { get; set; }
        public string ErrorEstadoPedido { get; set; }

        [TempData]
        public string MensajePedido { get; set; }

        [TempData]
        public string MensajeCambioEstado { get; set; }

        // Método privado para cargar datos de sesión
        private void CargarSesion()
        {
            idCliente = HttpContext.Session.GetInt32("ClienteId") ?? 0;

            var conexionIdStr = HttpContext.Session.GetString("ConexionId");
            ConexionId = int.TryParse(conexionIdStr, out int conexionId) ? conexionId : 0;
        }

        public void OnGet()
        {
            CargarSesion();
        }

        public async Task<IActionResult> OnPostDesconectarAsync()
        {
            CargarSesion();

            if (ConexionId > 0)
            {
                await _conexionService.DesconectarAsync(ConexionId);
                HttpContext.Session.Remove("ClienteId");
                HttpContext.Session.Remove("ConexionId");

                return RedirectToPage("/User/Index");
            }

            return BadRequest("No se pudo desconectar.");
        }

        public async Task<IActionResult> OnPostGenerarPedidoAsync()
        {
            CargarSesion();

            if (ConexionId <= 0)
            {
                MensajePedido = "Conexión inválida.";
                return Page();
            }

            try
            {
                int nuevoPedidoId = await _conexionService.GenerarPedidoAsync(ConexionId);
                MensajePedido = $"Pedido generado con ID: {nuevoPedidoId}";
            }
            catch (Exception ex)
            {
                MensajePedido = "Error al generar el pedido: " + ex.Message;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostConsultarEstadoPedidoAsync()
        {
            CargarSesion();

            if (ConexionId <= 0)
            {
                ErrorEstadoPedido = "El ID de conexión no es válido.";
                return Page();
            }

            if (PedidoId <= 0)
            {
                ErrorEstadoPedido = "El número de pedido no es válido.";
                return Page();
            }

            try
            {
                EstadoPedido = await _conexionService.ConsultarEstadoPedidoAsync(ConexionId, PedidoId);
            }
            catch (Exception ex)
            {
                ErrorEstadoPedido = "Error al consultar el estado del pedido: " + ex.Message;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostCambiarEstadoPedidoAsync()
        {
            CargarSesion();

            if (ConexionId <= 0 || PedidoId <= 0 || string.IsNullOrWhiteSpace(NuevoEstado))
            {
                MensajeCambioEstado = "Datos inválidos.";
                return Page();
            }

            MensajeCambioEstado = await _conexionService.CambiarEstadoPedidoAsync(ConexionId, PedidoId, NuevoEstado);
            return Page();
        }
    }
}
