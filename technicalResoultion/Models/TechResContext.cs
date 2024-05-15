using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace technicalResoultion.Models
{
    public class TechResContext : DbContext
    {
        public TechResContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<tickets> tickets { get; set; }
        public DbSet<estados> estados { get; set; }
        public DbSet<externos> externos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<tickets>().ToTable(tb => tb.HasTrigger("trg_fechas_estado_tickets"));
        }
    }
}
