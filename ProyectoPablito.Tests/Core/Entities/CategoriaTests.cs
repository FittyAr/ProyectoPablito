using FluentAssertions;
using ProyectoPablito.Core.Entities;
using Xunit;

namespace ProyectoPablito.Tests.Core.Entities;

public class CategoriaTests
{
    [Fact]
    public void NewCategoria_ShouldHaveId()
    {
        // Act
        var cat = new Categoria();

        // Assert
        cat.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Categoria_ShouldStoreProperties()
    {
        // Arrange
        var cat = new Categoria { Nombre = "Obras", Descripcion = "Gastos de obra" };

        // Assert
        cat.Nombre.Should().Be("Obras");
        cat.Descripcion.Should().Be("Gastos de obra");
    }
}
