using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using tp6_torres_zucchini.Data.Models;

namespace tp6_torres_zucchini.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<PedidoHistorial> PedidoHistoriales { get; set; }
        public DbSet<Conexion> Conexiones { get; set; }

        public DbSet<LogPeticion> LogPeticiones { get; set; }
    }
}
