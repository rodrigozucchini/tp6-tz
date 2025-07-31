﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tp6_torres_zucchini.Data.Models
{
    public class Conexion
    {
        [Key]
        public int Id { get; set; }

        public DateTime FechaHora { get; set; } = DateTime.UtcNow;

        public bool Activa { get; set; } = false;

        [ForeignKey("Cliente")]
        public int ClienteId { get; set; }
    }
}
