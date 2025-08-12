using System.ComponentModel.DataAnnotations;

namespace tp6_torres_zucchini.Data.Models
{
    public class Cliente
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Nombre { get; set; }

        [Required]
        [MaxLength(50)]
        public string Email { get; set; }

        public bool? Activo { get; set; }

    }

}
