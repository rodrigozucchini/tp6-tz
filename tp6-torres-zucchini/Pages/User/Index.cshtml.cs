using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using tp6_torres_zucchini.Data;
using tp6_torres_zucchini.Service;

namespace tp6_torres_zucchini.Pages.User
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IConexionService _conexionService; // Cambiado a la interfaz

        public IndexModel(ApplicationDbContext context, IConexionService conexionService)
        {
            _context = context;
            _conexionService = conexionService; // Inyección de la interfaz
        }

        [BindProperty]
        public int IdCliente { get; set; }

        public string ErrorMessage { get; set; } = string.Empty;
        public bool ShowForm { get; set; }

        public void OnGet()
        {
            ShowForm = false;
        }

        public IActionResult OnPostMostrarFormulario()
        {
            ShowForm = true;
            return Page();
        }

        // Método OnPostIngresar
        public async Task<IActionResult> OnPostIngresar()
        {
            ShowForm = true;

            try
            {
                var cliente = await _context.Clientes
                    .FirstOrDefaultAsync(c => c.Id == IdCliente);

                if (cliente == null)
                {
                    ErrorMessage = "No se encontró un cliente con esos datos.";
                    Response.StatusCode = 404;
                    return Page();
                }

                var conexionId = await _conexionService.ConectarAsync(cliente.Id);

                if (conexionId.StartsWith("ERROR"))
                {
                    ErrorMessage = conexionId;
                    Response.StatusCode = 400;
                    return Page();
                }

                // Guardar en Session en vez de TempData
                HttpContext.Session.SetInt32("ClienteId", cliente.Id);
                HttpContext.Session.SetString("ConexionId", conexionId);

                return RedirectToPage("./Pedidos");
            }
            catch (Exception ex)
            {
                ErrorMessage = "Ocurrió un error: " + ex.Message;
                Response.StatusCode = 500;
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            return await OnPostIngresar();
        }
    }
}