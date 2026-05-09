using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ProyectoPablito.Core.Entities;
using ProyectoPablito.Infrastructure.Data;
using ProyectoPablito.Infrastructure.Repositories;
using Xunit;

namespace ProyectoPablito.Tests.Infrastructure.Repositories;

public class ClienteRepositoryTests
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
    public async Task GetAllWithContactosAsync_ShouldLoadContactos()
    {
        // Arrange
        using var context = await GetDbContextAsync();
        var repository = new ClienteRepository(context);
        
        var cliente = new Cliente { Nombre = "Empresa X" };
        cliente.Contactos.Add(new ClienteContacto { Etiqueta = "Ventas", Email = "v@x.com" });
        context.Add(cliente);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllWithContactosAsync();

        // Assert
        var item = result.First();
        item.Contactos.Should().NotBeEmpty();
        item.Contactos.First().Etiqueta.Should().Be("Ventas");
    }
}
