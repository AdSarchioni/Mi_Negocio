
using Mi_Negocio.Models;
using Microsoft.EntityFrameworkCore;
namespace Mi_Negocio.Data

{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Producto> Productos { get; set; }
        
    }
}