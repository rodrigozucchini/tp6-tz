using global::tp6_torres_zucchini.Data.Models;
using Microsoft.EntityFrameworkCore;
using tp6_torres_zucchini.Service;

namespace tp6_torres_zucchini.Data
{
    public class ConexionService : IConexionService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogService _logService;

        public ConexionService(ApplicationDbContext context, ILogService logService)
        {
            _context = context;
            _logService = logService;
        }

        public async Task<string> ConectarAsync(int clienteId)
        {
            var comando = "Conectar";
            string respuesta;

            try
            {
                var clienteExiste = await _context.Clientes.AnyAsync(c => c.Id == clienteId);
                if (!clienteExiste)
                {
                    respuesta = "ERROR Cliente no encontrado";
                    await _logService.RegistrarPeticionAsync(comando, clienteId, respuesta);
                    return respuesta;
                }

                var conexionActivaExistente = await _context.Conexiones
                    .AnyAsync(c => c.ClienteId == clienteId && c.Activa);

                if (conexionActivaExistente)
                {
                    respuesta = "ERROR Ya existe una conexión activa para este cliente";
                    await _logService.RegistrarPeticionAsync(comando, clienteId, respuesta);
                    return respuesta;
                }

                var nuevaConexion = new Conexion
                {
                    ClienteId = clienteId,
                    FechaHora = DateTime.UtcNow,
                    Activa = true
                };

                _context.Conexiones.Add(nuevaConexion);
                await _context.SaveChangesAsync();

                respuesta = nuevaConexion.Id.ToString();
                await _logService.RegistrarPeticionAsync(comando, clienteId,
                    $"Conexión creada correctamente con Id {nuevaConexion.Id} para el cliente {clienteId}");

                return respuesta;
            }
            catch (Exception ex)
            {
                respuesta = $"ERROR {ex.Message}";
                await _logService.RegistrarPeticionAsync(comando, clienteId, respuesta);
                return respuesta;
            }
        }

        public async Task<string> DesconectarAsync(int conexionId)
        {
            var comando = "Desconectar";
            string respuesta;

            var conexion = await _context.Conexiones.FirstOrDefaultAsync(c => c.Id == conexionId);

            if (conexion == null)
            {
                respuesta = "ERROR (Conexión no encontrada)";
            }
            else if (!conexion.Activa) // ya está inactiva
            {
                respuesta = $"ERROR (La conexión {conexionId} ya está desconectada)";
            }
            else
            {
                conexion.Activa = false;
                await _context.SaveChangesAsync();
                respuesta = $"Conexión {conexionId} desconectada correctamente";
            }

            await _logService.RegistrarPeticionAsync(comando, conexionId, respuesta);
            return respuesta;
        }

        public async Task<string> ObtenerEstadoServidorAsync(int conexionId)
        {
            var comando = "EstadoServidor";
            string respuesta;

            var conexion = await _context.Conexiones.FirstOrDefaultAsync(c => c.Id == conexionId);
            respuesta = conexion != null && conexion.Activa ? "ACTIVO" : "INACTIVO";

            await _logService.RegistrarPeticionAsync(comando, conexionId,
                $"Estado del servidor para la conexión {conexionId}: {respuesta}");
            return respuesta;
        }

        public async Task<string> ObtenerFechaServidorAsync(int conexionId)
        {
            var comando = "FechaServidor";
            string respuesta;

            var conexion = await _context.Conexiones.FirstOrDefaultAsync(c => c.Id == conexionId);

            if (conexion == null)
            {
                respuesta = "INACTIVO";
            }
            else
            {
                respuesta = DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss");
            }

            await _logService.RegistrarPeticionAsync(comando, conexionId,
                $"Fecha del servidor para la conexión {conexionId}: {respuesta}");
            return respuesta;
        }

        public async Task<int> GenerarPedidoAsync(int conexionId)
        {
            var comando = "GenerarPedido";

            var conexion = await _context.Conexiones.FirstOrDefaultAsync(c => c.Id == conexionId);
            if (conexion == null)
            {
                var error = "ERROR (Conexión no encontrada)";
                await _logService.RegistrarPeticionAsync(comando, conexionId, error);
                throw new Exception(error);
            }

            var nuevoPedido = new Pedido
            {
                ClienteId = conexion.ClienteId,
                ConexionId = conexion.Id,
                Estado = "PENDIENTE"
            };

            _context.Pedidos.Add(nuevoPedido);
            await _context.SaveChangesAsync();

            var historial = new PedidoHistorial
            {
                PedidoId = nuevoPedido.Id,
                ClienteId = conexion.ClienteId,
                Estado = nuevoPedido.Estado
            };

            _context.PedidoHistoriales.Add(historial);
            await _context.SaveChangesAsync();

            await _logService.RegistrarPeticionAsync(comando, conexionId,
                $"Pedido {nuevoPedido.Id} generado para el cliente {conexion.ClienteId}");
            return nuevoPedido.Id;
        }

        public async Task<string> ConsultarEstadoPedidoAsync(int conexionId, int pedidoId)
        {
            var comando = "ConsultarEstado";
            string respuesta;

            var conexion = await _context.Conexiones.FirstOrDefaultAsync(c => c.Id == conexionId);
            if (conexion == null)
            {
                respuesta = "ERROR (Conexión no encontrada)";
                await _logService.RegistrarPeticionAsync(comando, conexionId, respuesta);
                throw new Exception(respuesta);
            }

            var pedido = await _context.Pedidos
                .FirstOrDefaultAsync(p => p.Id == pedidoId && p.ClienteId == conexion.ClienteId);

            if (pedido == null)
            {
                respuesta = "ERROR (Pedido no encontrado)";
                await _logService.RegistrarPeticionAsync(comando, conexionId, respuesta);
                throw new Exception(respuesta);
            }

            respuesta = pedido.Estado;
            await _logService.RegistrarPeticionAsync(comando, conexionId,
                $"Estado del pedido {pedidoId} para la conexión {conexionId}: {respuesta}");
            return respuesta;
        }

        public async Task<string> CambiarEstadoPedidoAsync(int conexionId, int pedidoId, string nuevoEstado)
        {
            var comando = "CambiarEstado";
            string respuesta;

            var estadosValidos = new[] { "PENDIENTE", "DESPACHADO", "ENTREGADO", "CERRADO", "ANULADO" };

            if (!estadosValidos.Contains(nuevoEstado.ToUpper()))
            {
                respuesta = $"ERROR (Estado: {nuevoEstado} no contemplado por el servidor)";
                await _logService.RegistrarPeticionAsync(comando, conexionId, respuesta);
                return respuesta;
            }

            var conexion = await _context.Conexiones.FirstOrDefaultAsync(c => c.Id == conexionId && c.Activa);
            if (conexion == null)
            {
                respuesta = "ERROR (Conexión no encontrada o inactiva)";
                await _logService.RegistrarPeticionAsync(comando, conexionId, respuesta);
                return respuesta;
            }

            var pedido = await _context.Pedidos.FirstOrDefaultAsync(p => p.Id == pedidoId && p.ClienteId == conexion.ClienteId);
            if (pedido == null)
            {
                respuesta = "ERROR (Pedido no encontrado)";
                await _logService.RegistrarPeticionAsync(comando, conexionId, respuesta);
                return respuesta;
            }

            pedido.Estado = nuevoEstado.ToUpper();
            _context.Pedidos.Update(pedido);

            var historial = new PedidoHistorial
            {
                PedidoId = pedido.Id,
                ClienteId = conexion.ClienteId,
                Estado = nuevoEstado.ToUpper(),
                FechaHora = DateTime.UtcNow
            };

            _context.PedidoHistoriales.Add(historial);

            await _context.SaveChangesAsync();

            respuesta = "OK";
            await _logService.RegistrarPeticionAsync(comando, conexionId,
                $"Pedido {pedidoId} para el cliente {conexion.ClienteId} cambiado a estado {nuevoEstado.ToUpper()}");
            return respuesta;
        }
    }
}
