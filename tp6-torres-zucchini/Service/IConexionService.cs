namespace tp6_torres_zucchini.Service
{
    public interface IConexionService
    {
        Task<string> ConectarAsync(int clienteId);                     // Conectar un cliente
        Task<string> ObtenerEstadoServidorAsync(int conexionId);        // Estado del servidor
        Task<string> ObtenerFechaServidorAsync(int conexionId);         // Fecha y hora del servidor
        Task<string> DesconectarAsync(int conexionId);                  // Desconectar un cliente
        
        //PEDIDOS
        Task<int>GenerarPedidoAsync(int clienteId);
        Task<string> ConsultarEstadoPedidoAsync(int conexionId, int pedidoId);
        Task<string> CambiarEstadoPedidoAsync(int conexionId, int pedidoId, string nuevoEstado);

    }
}
