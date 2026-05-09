using System;
using FluentAssertions;
using ProyectoPablito.Core.Entities;
using Xunit;

namespace ProyectoPablito.Tests.Core.Entities;

public class LiquidacionTests
{
    [Fact]
    public void TotalBruto_ShouldCalculateCorrectly()
    {
        // Arrange
        var liq = new Liquidacion
        {
            DiasTrabajados = 10.5m,
            TarifaAplicada = 1000m
        };

        // Assert
        liq.TotalBruto.Should().Be(10500m);
    }

    [Fact]
    public void TotalNeto_ShouldSubtractAdelantos()
    {
        // Arrange
        var liq = new Liquidacion
        {
            DiasTrabajados = 10,
            TarifaAplicada = 1000,
            TotalAdelantos = 2000
        };

        // Assert
        liq.TotalNeto.Should().Be(8000m);
    }
}
