using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tp6_torres_zucchini.Data.Models
{
    public class Pedido
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Estado { get; set; } = "PENDIENTE";

        [ForeignKey("Cliente")]
        public int ClienteId { get; set; }

        [ForeignKey("Conexion")]
        public int ConexionId { get; set; }

    }
}
