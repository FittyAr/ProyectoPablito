using FluentAssertions;
using ElectroObraApp.Core.Entities;
using Xunit;

namespace ElectroObraApp.Tests.Core.Entities;

public class MovimientoTests
{
    [Fact]
    public void Total_ShouldBeMontoMultipliedByCantidad()
    {
        // Arrange
        var mov = new Movimiento { Monto = 150, Cantidad = 2 };

        // Assert
        mov.Total.Should().Be(300);
    }

    [Fact]
    public void DefaultCantidad_ShouldBeOne()
    {
        // Act
        var mov = new Movimiento();

        // Assert
        mov.Cantidad.Should().Be(1);
    }
}

