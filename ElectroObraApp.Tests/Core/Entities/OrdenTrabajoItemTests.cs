using FluentAssertions;
using ElectroObraApp.Core.Entities;
using Xunit;

namespace ElectroObraApp.Tests.Core.Entities;

public class OrdenTrabajoItemTests
{
    [Fact]
    public void PorcentajeAcumulado_ShouldBeSumOfAnteriorAndActual()
    {
        // Arrange
        var item = new OrdenTrabajoItem { PorcentajeAnterior = 20, PorcentajeActual = 10 };

        // Assert
        item.PorcentajeAcumulado.Should().Be(30);
    }

    [Fact]
    public void SubtotalActual_ShouldCalculateBasedOnPercentage()
    {
        // Arrange
        var item = new OrdenTrabajoItem 
        { 
            Cantidad = 100, 
            PrecioUnitario = 10, 
            PorcentajeActual = 50 
        };

        // Act
        var result = item.SubtotalActual;

        // Assert
        // 100 * 10 * 0.5 = 500
        result.Should().Be(500);
    }

    [Fact]
    public void SubtotalAcumulado_ShouldCalculateBasedOnAccumulatedPercentage()
    {
        // Arrange
        var item = new OrdenTrabajoItem 
        { 
            Cantidad = 100, 
            PrecioUnitario = 10, 
            PorcentajeAnterior = 20,
            PorcentajeActual = 30 
        };

        // Act
        var result = item.SubtotalAcumulado;

        // Assert
        // 100 * 10 * (20+30)/100 = 500
        result.Should().Be(500);
    }
}

