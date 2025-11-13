using Microsoft.EntityFrameworkCore;
using BE_PE.Models;

namespace BE_PE.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Movie> Movies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Movie entity
            modelBuilder.Entity<Movie>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired();
                entity.HasIndex(e => e.Title);
                entity.HasIndex(e => e.Genre);
                entity.HasIndex(e => e.Rating);
            });
        }
    }
}
