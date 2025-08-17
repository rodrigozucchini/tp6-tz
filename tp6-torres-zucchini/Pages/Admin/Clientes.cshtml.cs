using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using tp6_torres_zucchini.Data;
using tp6_torres_zucchini.Data.Models;

namespace tp6_torres_zucchini.Pages.Admin
{
    public class ClientesModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public ClientesModel(ApplicationDbContext context) { _context = context; }

        [BindProperty]
        public Cliente ClienteActual { get; set; }

        // Flags para controlar UI
        public bool InputsEditables { get; set; }
        public bool InputID { get; set; }
        public bool MostrarBotonGuardar { get; set; }
        public bool MostrarBotonEliminar { get; set; }
        public bool MostrarBotonModificar { get; set; }
        public bool MostrarBotonLimpiar { get; set; }
        public bool MostrarBotonCancelar { get; set; }

        public void OnGet()
        {
            ClienteActual = new Cliente();
            InputsEditables = false;
            InputID = true;  // ID editable para buscar
            MostrarBotonGuardar = false;
            MostrarBotonEliminar = false;
            MostrarBotonModificar = false;
            MostrarBotonLimpiar = true;
            MostrarBotonCancelar = false;
        }

        public async Task<IActionResult> OnPostBuscarAsync()
        {
            ModelState.Clear();
            if (ClienteActual?.Id > 0)
            {
                ClienteActual = await _context.Clientes.FindAsync(ClienteActual.Id);

                if (ClienteActual != null && ClienteActual.Activo == true)
                {
                    InputsEditables = false;      // Nombre y Email no editables
                    InputID = true;               // ID editable para buscar otro
                    MostrarBotonModificar = true;
                    MostrarBotonEliminar = true;
                    MostrarBotonGuardar = false;
                    MostrarBotonLimpiar = true;
                }
                else
                {
                    ModelState.AddModelError("", "Cliente no encontrado");
                    ClienteActual = new Cliente();
                    InputsEditables = false;
                    InputID = true;
                    MostrarBotonLimpiar = true;
                }
            }
            return Page();
        }


        public IActionResult OnPostAlta()
        {
           // ModelState.Clear();
            ClienteActual = new Cliente();
            InputsEditables = true;
            InputID = false; // ID inhabilitado
            MostrarBotonGuardar = true;
            MostrarBotonEliminar = false;
            MostrarBotonModificar = false;
            MostrarBotonLimpiar = true;
            MostrarBotonCancelar = false;
            return Page();
        }

        public async Task<IActionResult> OnPostGuardarAsync()
        {
            if (!ModelState.IsValid) return Page();

            if (ClienteActual.Id == 0)
            {
                ClienteActual.Activo = true; // Solo al crear un nuevo cliente
            _context.Clientes.Add(ClienteActual);
            }
            else
                _context.Clientes.Update(ClienteActual);

            await _context.SaveChangesAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEliminarAsync()
        {
            var cliente = await _context.Clientes.FindAsync(ClienteActual.Id);
            if (cliente != null)
            {
                cliente.Activo = false; // Marcar como inactivo
                _context.Clientes.Update(cliente);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        public IActionResult OnPostModificar()
        {
            InputsEditables = true;
            InputID = false; // ID inhabilitado al modificar
            MostrarBotonGuardar = true;
            MostrarBotonEliminar = false;
            MostrarBotonModificar = false;
            MostrarBotonLimpiar = true;
            MostrarBotonCancelar = true;
            return Page();
        }

        public IActionResult OnPostCancelar()
        {
            if (ClienteActual?.Id > 0)
            {
                ClienteActual = _context.Clientes.Find(ClienteActual.Id);
                InputsEditables = false;
                InputID = true;
                MostrarBotonGuardar = false;
                MostrarBotonModificar = true;
                MostrarBotonEliminar = true;
                MostrarBotonLimpiar = true;
                MostrarBotonCancelar = false;
            }
            return Page();
        }

        public IActionResult OnPostLimpiar()
        {
            ClienteActual = new Cliente();
            InputsEditables = true;
            InputID = false;
            MostrarBotonGuardar = true;
            MostrarBotonEliminar = false;
            MostrarBotonModificar = false;
            MostrarBotonLimpiar = true;
            MostrarBotonCancelar = false;
            return RedirectToPage();
        }
    }
}


