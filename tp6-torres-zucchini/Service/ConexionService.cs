using global::tp6_torres_zucchini.Data.Models;
using Microsoft.EntityFrameworkCore;
using tp6_torres_zucchini.Service;

namespace tp6_torres_zucchini.Data
{
    public class ConexionService : IConexionService
    {
        private readonly ApplicationDbContext _context;

        public ConexionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> ConectarAsync(int clienteId)
        {
            // Desactivar conexiones activas con más de 5 minutos
            var limiteTiempo = DateTime.UtcNow.AddMinutes(-5);

            var conexionesExpiradas = await _context.Conexiones
                .Where(c => c.Activa && c.FechaHora <= limiteTiempo)
                .ToListAsync();

            if (conexionesExpiradas.Any())
            {
                foreach (var c in conexionesExpiradas)
                {
                    c.Activa = false;
                }
                await _context.SaveChangesAsync();
            }

            // Luego seguís con el resto de tu lógica, por ejemplo:
            var clienteExiste = await _context.Clientes.AnyAsync(c => c.Id == clienteId);
            if (!clienteExiste)
                return "ERROR Cliente no encontrado";

            var conexionActivaExistente = await _context.Conexiones
                .AnyAsync(c => c.ClienteId == clienteId && c.Activa);

            if (conexionActivaExistente)
                return "ERROR Ya existe una conexión activa para este cliente";

            var nuevaConexion = new Conexion
            {
                ClienteId = clienteId,
                FechaHora = DateTime.UtcNow,
                Activa = true
            };

            _context.Conexiones.Add(nuevaConexion);
            await _context.SaveChangesAsync();

            return nuevaConexion.Id.ToString();
        }

        public async Task<string> DesconectarAsync(int conexionId)
        {
            var conexion = await _context.Conexiones.FirstOrDefaultAsync(c => c.Id == conexionId);

            if (conexion == null)
            {
                return "ERROR (Conexión no encontrada)";
            }

            conexion.Activa = false;
            await _context.SaveChangesAsync();

            return "OK";
        }

        public async Task<string> ObtenerEstadoServidorAsync(int conexionId)
        {
            var conexion = await _context.Conexiones.FirstOrDefaultAsync(c => c.Id == conexionId);

            //if (conexion == null)
            //{
            //    return "INACTIVO";
            //}

            //return conexion.Activa ? "ACTIVO" : "INACTIVO";
            return "ACTIVO";
        }

        public async Task<string> ObtenerFechaServidorAsync(int conexionId)
        {
            var conexion = await _context.Conexiones.FirstOrDefaultAsync(c => c.Id == conexionId);

            if (conexion == null)
            {
                return "INACTIVO";
            }

            return DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss");
        }

        public async Task<int> GenerarPedidoAsync(int conexionId)
        {
            var conexion = await _context.Conexiones
                    .FirstOrDefaultAsync(c => c.Id == conexionId);

            if (conexion == null)
            {
                throw new Exception("Conexión no encontrada");
            }

            var nuevoPedido = new Pedido
            {
                ClienteId = conexion.ClienteId,
                ConexionId = conexion.Id,
                Estado = "PENDIENTE"
            };

            _context.Pedidos.Add(nuevoPedido);
            await _context.SaveChangesAsync();

            // Guardar historial
            var historial = new PedidoHistorial
            {
                PedidoId = nuevoPedido.Id,
                ClienteId = conexion.ClienteId,
                Estado = nuevoPedido.Estado
            };

            _context.PedidoHistoriales.Add(historial);
            await _context.SaveChangesAsync();

            return nuevoPedido.Id;
        }

        public async Task<string> ConsultarEstadoPedidoAsync(int conexionId, int pedidoId)
        {
            // Buscar la conexión
            var conexion = await _context.Conexiones.FirstOrDefaultAsync(c => c.Id == conexionId);

            if (conexion == null)
                throw new Exception("Conexión no encontrada");

            // Buscar el pedido que pertenezca al cliente de esta conexión y con ese pedidoId
            var pedido = await _context.Pedidos
                .FirstOrDefaultAsync(p => p.Id == pedidoId && p.ClienteId == conexion.ClienteId);

            if (pedido == null)
                throw new Exception("Pedido no encontrado");

            // Retornar el estado actual
            return pedido.Estado;
        }

        public async Task<string> CambiarEstadoPedidoAsync(int conexionId, int pedidoId, string nuevoEstado)
        {
            // Estados válidos
            var estadosValidos = new[] { "PENDIENTE", "DESPACHADO", "ENTREGADO", "CERRADO", "ANULADO" };

            // Validar estado
            if (!estadosValidos.Contains(nuevoEstado.ToUpper()))
            {
                return $"ERROR (Estado: {nuevoEstado} no contemplado por el servidor)";
            }

            // Buscar la conexión
            var conexion = await _context.Pedidos
                .FirstOrDefaultAsync(c => c.ConexionId == conexionId);

            if (conexion == null)
            {
                return "ERROR (Conexión no encontrada o inactiva)";
            }

            // Buscar el pedido
            var pedido = await _context.Pedidos
                .FirstOrDefaultAsync(p => p.Id == pedidoId);

            if (pedido == null)
            {
                return "ERROR (Pedido no encontrado)";
            }

            // Cambiar estado del pedido
            pedido.Estado = nuevoEstado.ToUpper();
            _context.Pedidos.Update(pedido);

            // Guardar en historial
            var historial = new PedidoHistorial
            {
                PedidoId = pedido.Id,
                ClienteId = conexion.ClienteId, // quien lo modifica
                Estado = nuevoEstado.ToUpper(),
                FechaHora = DateTime.UtcNow
            };

            _context.PedidoHistoriales.Add(historial);

            await _context.SaveChangesAsync();

            return "OK";
        }
    }
}
