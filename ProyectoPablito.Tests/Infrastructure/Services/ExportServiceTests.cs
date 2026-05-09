using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using ProyectoPablito.Application.DTOs;
using ProyectoPablito.Infrastructure.Services;
using Xunit;

namespace ProyectoPablito.Tests.Infrastructure.Services;

public class ExportServiceTests
{
    private readonly ExportService _service;

    public ExportServiceTests()
    {
        _service = new ExportService();
    }

    [Fact]
    public async Task ExportMovimientosToPdfAsync_ShouldReturnBytes()
    {
        // Arrange
        var data = new List<MovimientoDto> { new() { Concepto = "Test", Total = 100 } };

        // Act
        var result = await _service.ExportMovimientosToPdfAsync(data);

        // Assert
        result.Should().NotBeNull();
        result.Length.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task ExportMovimientosToExcelAsync_ShouldReturnBytes()
    {
        // Arrange
        var data = new List<MovimientoDto> { new() { Concepto = "Test", Total = 100 } };

        // Act
        var result = await _service.ExportMovimientosToExcelAsync(data);

        // Assert
        result.Should().NotBeNull();
        result.Length.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task ExportLiquidacionToPdfAsync_ShouldReturnBytes()
    {
        // Arrange
        var liq = new LiquidacionDto { EmpleadoNombre = "Juan", TotalNeto = 5000 };
        var adelantos = new List<MovimientoDto>();

        // Act
        var result = await _service.ExportLiquidacionToPdfAsync(liq, adelantos);

        // Assert
        result.Should().NotBeNull();
        result.Length.Should().BeGreaterThan(0);
    }
}
