using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ElectroObraApp.Core.Entities;
using ElectroObraApp.Infrastructure.Data;
using ElectroObraApp.Infrastructure.Repositories;
using Xunit;

namespace ElectroObraApp.Tests.Infrastructure.Repositories;

public class MovimientoRepositoryTests
{
    private async Task<ApplicationDbContext> GetDbContextAsync()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options;

        var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();
        
        return context;
    }

    [Fact]
    public async Task GetAllWithIncludesAsync_ShouldLoadRelatedEntities()
    {
        // Arrange
        using var context = await GetDbContextAsync();
        var repository = new MovimientoRepository(context);
        
        var tipo = new TipoMovimiento { Nombre = "Ingreso" };
        var categoria = new Categoria { Nombre = "Ventas" };
        context.Add(tipo);
        context.Add(categoria);
        await context.SaveChangesAsync();

        var movimiento = new Movimiento 
        { 
            Concepto = "Test", 
            Monto = 100, 
            TipoMovimientoId = tipo.Id,
            CategoriaId = categoria.Id
        };
        await repository.AddAsync(movimiento);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllWithIncludesAsync();

        // Assert
        var item = result.First();
        item.TipoMovimiento.Should().NotBeNull();
        item.TipoMovimiento.Nombre.Should().Be("Ingreso");
        item.Categoria.Should().NotBeNull();
        item.Categoria!.Nombre.Should().Be("Ventas");
    }

    [Fact]
    public async Task GetByIdWithIncludesAsync_ShouldReturnEntityWithRelated()
    {
        // Arrange
        using var context = await GetDbContextAsync();
        var repository = new MovimientoRepository(context);
        
        var tipo = new TipoMovimiento { Nombre = "Gasto" };
        context.Add(tipo);
        await context.SaveChangesAsync();

        var movimiento = new Movimiento 
        { 
            Concepto = "Gasto 1", 
            Monto = 50, 
            TipoMovimientoId = tipo.Id 
        };
        await repository.AddAsync(movimiento);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdWithIncludesAsync(movimiento.Id);

        // Assert
        result.Should().NotBeNull();
        result!.TipoMovimiento.Should().NotBeNull();
        result.TipoMovimiento.Nombre.Should().Be("Gasto");
    }
}

