using Microsoft.EntityFrameworkCore;
using TC_Api.Models;

namespace TC_Api
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Tarjeta> Tarjetas { get; set; }
    }
}
