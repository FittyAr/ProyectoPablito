using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ElectroObraApp.Core.Entities;
using ElectroObraApp.Infrastructure.Data;
using ElectroObraApp.Infrastructure.Repositories;
using Xunit;

namespace ElectroObraApp.Tests.Infrastructure.Repositories;

public class UnitOfWorkTests
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
    public async Task SaveChangesAsync_ShouldPersistData()
    {
        // Arrange
        using var context = await GetDbContextAsync();
        var uow = new UnitOfWork(context);
        var repo = uow.Repository<Categoria>();
        var categoria = new Categoria { Nombre = "Test" };

        // Act
        await repo.AddAsync(categoria);
        await uow.SaveChangesAsync();

        // Assert
        var result = await repo.GetByIdAsync(categoria.Id);
        result.Should().NotBeNull();
        result!.Nombre.Should().Be("Test");
    }
}

