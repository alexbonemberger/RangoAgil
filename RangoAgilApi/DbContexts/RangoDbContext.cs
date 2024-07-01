using Microsoft.EntityFrameworkCore;
using RangoAgilApi.Entities;

namespace RangoAgilApi.DbContexts
{
    public class RangoDbContext(DbContextOptions<RangoDbContext> options) : DbContext(options)
    {
        public DbSet<Rango> Rangos { get; set; } = null!;
        public DbSet<Ingrediente> Ingredientes { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<Ingrediente>().HasData(
                new { Id = 1, Name = "Pão" },
                new { Id = 2, Name = "Carne de Vaca" },
                new { Id = 3, Name = "Cebola" },
                new { Id = 4, Name = "Maionese" },
                new { Id = 5, Name = "Alface" }
            );

            _ = modelBuilder.Entity<Rango>().HasData(
                new { Id = 1, Name = "Subway" },
                new { Id = 2, Name = "Pizza" },
                new { Id = 3, Name = "Sopa" },
                new { Id = 4, Name = "Espaguete" },
                new { Id = 5, Name = "Lasanha" }
            );

            _ = modelBuilder
                .Entity<Rango>()
                .HasMany(i => i.Ingredientes)
                .WithMany(r => r.Rangos)
                .UsingEntity(e => e.HasData(
                    new { RangosId = 1, IngredientesId = 1 },
                    new { RangosId = 1, IngredientesId = 2 },
                    new { RangosId = 2, IngredientesId = 1 },
                    new { RangosId = 2, IngredientesId = 3 },
                    new { RangosId = 3, IngredientesId = 1 },
                    new { RangosId = 3, IngredientesId = 5 },
                    new { RangosId = 3, IngredientesId = 2 },
                    new { RangosId = 4, IngredientesId = 1 },
                    new { RangosId = 4, IngredientesId = 3 },
                    new { RangosId = 5, IngredientesId = 5 }
                ));

            base.OnModelCreating(modelBuilder);
        }

    }
}
