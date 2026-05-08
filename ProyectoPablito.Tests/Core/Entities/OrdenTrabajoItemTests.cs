using FluentAssertions;
using ProyectoPablito.Core.Entities;
using Xunit;

namespace ProyectoPablito.Tests.Core.Entities;

public class OrdenTrabajoItemTests
{
    [Fact]
    public void OrdenTrabajoItem_CalculatedFields_ShouldBeCorrect()
    {
        // Arrange
        var item = new OrdenTrabajoItem
        {
            Descripcion = "Test Item",
            Cantidad = 10,
            PrecioUnitario = 1000,
            PorcentajeAnterior = 20,
            PorcentajeActual = 30
        };

        // Act & Assert
        item.PorcentajeAcumulado.Should().Be(50);
        
        // Subtotal Actual: 10 * 1000 * 0.30 = 3000
        item.SubtotalActual.Should().Be(3000);
        
        // Subtotal Acumulado: 10 * 1000 * 0.50 = 5000
        item.SubtotalAcumulado.Should().Be(5000);
    }
}
