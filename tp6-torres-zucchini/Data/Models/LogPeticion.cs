using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tp6_torres_zucchini.Data.Models
{
    public class LogPeticion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Comando { get; set; }

        [ForeignKey("Conexion")]
        public int ConexionId { get; set; }

        [Required]
        [MaxLength(50)]
        public string RespuestaComando { get; set; }

        public DateTime FechaHora { get; set; } = DateTime.UtcNow;

    }
}
