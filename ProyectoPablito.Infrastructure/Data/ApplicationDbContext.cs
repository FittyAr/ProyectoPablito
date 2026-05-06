using Microsoft.EntityFrameworkCore;
using ProyectoPablito.Core.Entities;

namespace ProyectoPablito.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Movimiento> Movimientos => Set<Movimiento>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<TipoMovimiento> TipoMovimientos => Set<TipoMovimiento>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Conversión de Decimal a Double para SQLite
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var properties = entityType.GetProperties()
                .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?));

            foreach (var property in properties)
            {
                modelBuilder.Entity(entityType.ClrType).Property(property.Name).HasConversion<double>();
            }
        }

        // Configuraciones adicionales
        modelBuilder.Entity<Movimiento>(entity =>
        {
            entity.HasOne(x => x.Categoria)
                .WithMany(x => x.Movimientos)
                .HasForeignKey(x => x.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.TipoMovimiento)
                .WithMany(x => x.Movimientos)
                .HasForeignKey(x => x.TipoMovimientoId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Seed Data para Tipos de Movimiento
        var systemDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        modelBuilder.Entity<TipoMovimiento>().HasData(
            new TipoMovimiento { Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), Nombre = "Ingreso", EsIngreso = true, EsSistema = true, CreatedAt = systemDate },
            new TipoMovimiento { Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), Nombre = "Gasto", EsIngreso = false, EsSistema = true, CreatedAt = systemDate },
            new TipoMovimiento { Id = Guid.Parse("00000000-0000-0000-0000-000000000003"), Nombre = "Adelanto", EsIngreso = false, EsSistema = true, CreatedAt = systemDate },
            new TipoMovimiento { Id = Guid.Parse("00000000-0000-0000-0000-000000000004"), Nombre = "Ajuste", EsIngreso = true, EsSistema = true, CreatedAt = systemDate }
        );
    }
}
