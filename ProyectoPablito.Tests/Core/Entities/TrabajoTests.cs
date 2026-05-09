using FluentAssertions;
using ProyectoPablito.Core.Entities;
using Xunit;

namespace ProyectoPablito.Tests.Core.Entities;

public class TrabajoTests
{
    [Fact]
    public void NewTrabajo_ShouldInitializeCollections()
    {
        // Act
        var trabajo = new Trabajo();

        // Assert
        trabajo.GastosEIngresos.Should().NotBeNull();
        trabajo.OrdenesTrabajo.Should().NotBeNull();
    }

    [Fact]
    public void Trabajo_ShouldStoreProperties()
    {
        // Arrange
        var trabajo = new Trabajo 
        { 
            Descripcion = "Reparación", 
            Presupuesto = 10000 
        };

        // Assert
        trabajo.Descripcion.Should().Be("Reparación");
        trabajo.Presupuesto.Should().Be(10000);
    }
}
