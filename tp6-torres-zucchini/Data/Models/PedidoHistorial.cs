using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tp6_torres_zucchini.Data.Models
{
    public class PedidoHistorial
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Pedido")]
        public int PedidoId { get; set; }

        [ForeignKey("Cliente")]
        public int ClienteId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Estado { get; set; }

        public DateTime FechaHora { get; set; } = DateTime.UtcNow;
    }
}
