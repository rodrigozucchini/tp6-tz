using System.Threading.Tasks;
using tp6_torres_zucchini.Data.Models;

namespace tp6_torres_zucchini.Service
{
    public interface ILogService
    {
        Task RegistrarPeticionAsync(string comando, int? conexionId, string respuesta);
    }
}