using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tp6_torres_zucchini.Data;
using tp6_torres_zucchini.Data.Models;

namespace tp6_torres_zucchini.Controllers
{
    [ApiController]
    [Route("admin/[controller]")]
    [ApiExplorerSettings(GroupName = "PanelAdmin")]
    public class AdminConexion : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminConexion(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/AdminConexion
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Conexion>>> GetConexiones()
        {
            return await _context.Conexiones.ToListAsync();
        }

        // GET: api/AdminConexion/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Conexion>> GetConexion(int id)
        {
            var conexion = await _context.Conexiones.FindAsync(id);

            if (conexion == null)
            {
                return NotFound();
            }

            return conexion;
        }

        // PUT: api/AdminConexion/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutConexion(int id, Conexion conexion)
        {
            if (id != conexion.Id)
            {
                return BadRequest();
            }

            _context.Entry(conexion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ConexionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/AdminConexion
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Conexion>> PostConexion(Conexion conexion)
        {
            _context.Conexiones.Add(conexion);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetConexion", new { id = conexion.Id }, conexion);
        }

        // DELETE: api/AdminConexion/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConexion(int id)
        {
            var conexion = await _context.Conexiones.FindAsync(id);
            if (conexion == null)
            {
                return NotFound();
            }

            _context.Conexiones.Remove(conexion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ConexionExists(int id)
        {
            return _context.Conexiones.Any(e => e.Id == id);
        }
    }
}
