using FluentAssertions;
using ProyectoPablito.Core.Entities;
using Xunit;

namespace ProyectoPablito.Tests.Core.Entities;

public class OrdenTrabajoTests
{
    [Fact]
    public void NewOrdenTrabajo_ShouldInitializeItems()
    {
        // Act
        var orden = new OrdenTrabajo();

        // Assert
        orden.Items.Should().NotBeNull();
    }

    [Fact]
    public void OrdenTrabajo_ShouldStoreProperties()
    {
        // Arrange
        var orden = new OrdenTrabajo { Titulo = "Certificado 1", AjusteUocraPorcentaje = 8 };

        // Assert
        orden.Titulo.Should().Be("Certificado 1");
        orden.AjusteUocraPorcentaje.Should().Be(8);
    }
}
