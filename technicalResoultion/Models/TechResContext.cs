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
        public DbSet<internos> internos { get; set; }
        public DbSet<roles> roles { get; set; }
        public DbSet<asignar_tareas> asignar_tareas { get; set; }
        public DbSet<comentarios> comentarios { get; set; }
        public DbSet<dashboard> dashboard { get; set; }
        public DbSet<categorias> categorias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<tickets>().ToTable(tb => tb.HasTrigger("trg_fechas_estado_tickets"));
            modelBuilder.Entity<asignar_tareas>().ToTable(tb => tb.HasTrigger("trg_estado_asignar_tarea_tickets"));
            modelBuilder.Entity<asignar_tareas>().ToTable(tb => tb.HasTrigger("trg_estado_asignar_tarea"));
            modelBuilder.Entity<comentarios>().ToTable(tb => tb.HasTrigger("trg_fecha_comentarios"));
        }
    }
}
