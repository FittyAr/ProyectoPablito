using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ProyectoPablito.Core.Entities;
using ProyectoPablito.Infrastructure.Data;
using ProyectoPablito.Infrastructure.Repositories;
using Xunit;

namespace ProyectoPablito.Tests.Infrastructure.Repositories;

public class RepositoryTests
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
    public async Task AddAsync_ShouldAddEntityToDatabase()
    {
        // Arrange
        using var context = await GetDbContextAsync();
        var repository = new Repository<Categoria>(context);
        var categoria = new Categoria { Nombre = "Nueva" };

        // Act
        await repository.AddAsync(categoria);
        await context.SaveChangesAsync();

        // Assert
        var result = await repository.GetByIdAsync(categoria.Id);
        result.Should().NotBeNull();
        result!.Nombre.Should().Be("Nueva");
    }

    [Fact]
    public async Task Update_ShouldModifyExistingEntity()
    {
        // Arrange
        using var context = await GetDbContextAsync();
        var repository = new Repository<Categoria>(context);
        var categoria = new Categoria { Nombre = "Original" };
        await repository.AddAsync(categoria);
        await context.SaveChangesAsync();

        // Act
        categoria.Nombre = "Modificada";
        repository.Update(categoria);
        await context.SaveChangesAsync();

        // Assert
        var result = await repository.GetByIdAsync(categoria.Id);
        result!.Nombre.Should().Be("Modificada");
    }

    [Fact]
    public async Task Remove_ShouldRemoveEntity()
    {
        // Arrange
        using var context = await GetDbContextAsync();
        var repository = new Repository<Categoria>(context);
        var categoria = new Categoria { Nombre = "ABorrar" };
        await repository.AddAsync(categoria);
        await context.SaveChangesAsync();

        // Act
        repository.Remove(categoria);
        await context.SaveChangesAsync();

        // Assert
        var result = await repository.GetByIdAsync(categoria.Id);
        result.Should().BeNull();
    }
}
