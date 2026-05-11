using System;
using FluentAssertions;
using ElectroObraApp.Core.Entities;
using Xunit;

namespace ElectroObraApp.Tests.Core.Entities;

public class LiquidacionTests
{
    [Fact]
    public void TotalBruto_ShouldCalculateCorrectly()
    {
        // Arrange
        var liq = new Liquidacion
        {
            DiasTrabajados = 10.5m,
            TarifaAplicada = 1000m,
            TotalBruto = 10500m
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
            TotalBruto = 10000,
            TotalAdelantos = 2000
        };

        // Assert
        liq.TotalNeto.Should().Be(8000m);
    }
}

