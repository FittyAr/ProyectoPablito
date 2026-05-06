using FluentAssertions;
using ProyectoPablito.Core.Entities;
using ProyectoPablito.Core.Enums;
using Xunit;

namespace ProyectoPablito.Tests.Core.Entities;

public class MovimientoTests
{
    [Fact]
    public void Total_ShouldCalculateCorrectly_WhenMontoAndCantidadAreProvided()
    {
        // Arrange
        var movimiento = new Movimiento
        {
            Monto = 100.50m,
            Cantidad = 2
        };

        // Act
        var total = movimiento.Total;

        // Assert
        total.Should().Be(201.00m);
    }

    [Fact]
    public void NewMovimiento_ShouldHaveDefaultValues()
    {
        // Act
        var movimiento = new Movimiento();

        // Assert
        movimiento.Id.Should().NotBeEmpty();
        movimiento.Moneda.Should().Be(Moneda.ARS);
        movimiento.Cantidad.Should().Be(1);
    }
}
