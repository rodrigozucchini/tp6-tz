using tp6_torres_zucchini.Data;
using tp6_torres_zucchini.Data.Models;

namespace tp6_torres_zucchini.Service
{
    public class LogService : ILogService
    {
        private readonly ApplicationDbContext _context;

        public LogService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task RegistrarPeticionAsync(string comando, int? conexionId, string respuesta)
        {
            var log = new LogPeticion
            {
                Comando = comando,
                ConexionId = conexionId ?? 0, // si no hay cliente asociado
                RespuestaComando = respuesta,
                FechaHora = DateTime.UtcNow
            };

            _context.LogPeticion.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}