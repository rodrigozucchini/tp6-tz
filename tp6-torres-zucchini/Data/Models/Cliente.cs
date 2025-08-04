using System.ComponentModel.DataAnnotations;

namespace tp6_torres_zucchini.Data.Models
{
    public class Cliente
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        public string Email { get; set; }

        public bool? Activo { get; set; }

    }

}
