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

        public async Task<IActionResult> OnPostDesconectarAsync()
        {
            var conexionId = HttpContext.Session.GetString("ConexionId");
            if (!string.IsNullOrEmpty(conexionId))
            {
                await _conexionService.DesconectarAsync(int.Parse(conexionId));
                // Limpiar sesión
                HttpContext.Session.Remove("ClienteId");
                HttpContext.Session.Remove("ConexionId");

                return RedirectToPage("/User/Index");
            }
            return BadRequest("No se pudo desconectar.");
        }


        [TempData]
        public string MensajePedido { get; set; }

        public async Task<IActionResult> OnPostGenerarPedidoAsync()
        {
            var conexionIdStr = HttpContext.Session.GetString("ConexionId");
            if (string.IsNullOrEmpty(conexionIdStr) || !int.TryParse(conexionIdStr, out int conexionId))
            {
                MensajePedido = "Conexión inválida.";
                return Page();
            }

            try
            {
                int nuevoPedidoId = await _conexionService.GenerarPedidoAsync(conexionId);
                MensajePedido = $"Pedido generado con ID: {nuevoPedidoId}";
            }
            catch (Exception ex)
            {
                MensajePedido = "Error al generar el pedido: " + ex.Message;
            }

            return Page();
        }

        [BindProperty]
        public int ConexionId { get; set; }

        [BindProperty]
        public int PedidoId { get; set; }

        public string EstadoPedido { get; set; }
        public string ErrorEstadoPedido { get; set; }

        public async Task<IActionResult> OnPostConsultarEstadoPedidoAsync()
        {
            // Validar inputs (puedes agregar más validaciones)
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

        [BindProperty]
        public string NuevoEstado { get; set; }

        [TempData]
        public string MensajeCambioEstado { get; set; }

        public async Task<IActionResult> OnPostCambiarEstadoPedidoAsync()
        {
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
