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

public class TrabajoRepositoryTests
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
    public async Task GetAllWithDeepLoadAsync_ShouldLoadAllRelations()
    {
        // Arrange
        using var context = await GetDbContextAsync();
        var repository = new TrabajoRepository(context);
        
        var cliente = new Cliente { Nombre = "Cliente X" };
        var trabajo = new Trabajo { Descripcion = "Trabajo X", Cliente = cliente };
        var orden = new OrdenTrabajo { Titulo = "Orden 1", Trabajo = trabajo };
        orden.Items.Add(new OrdenTrabajoItem { Descripcion = "Item 1", OrdenTrabajo = orden });
        trabajo.OrdenesTrabajo.Add(orden);
        
        context.Add(trabajo);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllWithDeepLoadAsync();

        // Assert
        var item = result.First();
        item.Cliente.Should().NotBeNull();
        item.OrdenesTrabajo.Should().NotBeEmpty();
        item.OrdenesTrabajo.First().Items.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetByIdWithDeepLoadAsync_ShouldReturnEntityWithRelations()
    {
        // Arrange
        using var context = await GetDbContextAsync();
        var repository = new TrabajoRepository(context);
        
        var cliente = new Cliente { Nombre = "Cliente Y" };
        var trabajo = new Trabajo { Descripcion = "Trabajo Y", Cliente = cliente };
        context.Add(trabajo);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdWithDeepLoadAsync(trabajo.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Cliente.Should().NotBeNull();
        result.Cliente.Nombre.Should().Be("Cliente Y");
    }
}

