using Microsoft.EntityFrameworkCore;
using MuscleMetrics.Entidades;

namespace MuscleMetrics.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet <Usuario> Usuario { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=DESKTOP-3PF8N45\\SQLEXPRESS;Database=MuscleMetrics;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true;Encrypt=True;Integrated Security=True;");
        }
    }

}
