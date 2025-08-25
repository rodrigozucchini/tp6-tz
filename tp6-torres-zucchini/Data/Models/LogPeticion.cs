using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tp6_torres_zucchini.Data.Models
{
    public class LogPeticion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)] // aumenté un poco para más flexibilidad
        public string Comando { get; set; }

        [ForeignKey("Conexion")]
        public int? ConexionId { get; set; } // nullable por "si corresponde"

        [Required]
        [MaxLength(500)] // o quitar el MaxLength y dejarlo ilimitado
        public string RespuestaComando { get; set; }

        [Required]
        public DateTime FechaHora { get; set; } = DateTime.Now;
    }
}
