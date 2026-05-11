using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using ElectroObraApp.Application.Interfaces;
using ElectroObraApp.Application.Services;
using ElectroObraApp.Core.Entities;
using ElectroObraApp.Core.Interfaces;
using Xunit;

namespace ElectroObraApp.Tests.Application.Services;

public class LiquidacionServiceTests
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<LiquidacionService> _logger;
    private readonly LiquidacionService _service;

    public LiquidacionServiceTests()
    {
        _uow = Substitute.For<IUnitOfWork>();
        _logger = Substitute.For<ILogger<LiquidacionService>>();
        _service = new LiquidacionService(_uow, _logger);
    }

    [Fact]
    public async Task SugerirLiquidacionAsync_ShouldCalculateCorrectTotals_ExcludingWeekends()
    {
        // Arrange
        var empleadoId = Guid.NewGuid();
        // 2026-05-01 (Viernes) al 2026-05-15 (Viernes)
        // Dias: 1(V), 2(S), 3(D), 4(L), 5(M), 6(M), 7(J), 8(V), 9(S), 10(D), 11(L), 12(M), 13(M), 14(J), 15(V)
        // Habiles (L-V): 1, 4, 5, 6, 7, 8, 11, 12, 13, 14, 15 => Total 11 días
        var inicio = new DateTime(2026, 5, 1);
        var fin = new DateTime(2026, 5, 15);
        var tarifaDiaria = 40000m;

        var empleado = new Empleado { Id = empleadoId, Nombre = "Juan Perez", TarifaDiaria = tarifaDiaria };
        _uow.Repository<Empleado>().GetByIdAsync(empleadoId).Returns(empleado);

        var adelantoTypeId = Guid.Parse("00000000-0000-0000-0000-000000000003");
        var adelantos = new List<Movimiento>
        {
            new() { Id = Guid.NewGuid(), TipoMovimientoId = adelantoTypeId, Monto = 50000, Cantidad = 1, Fecha = inicio.AddDays(2) },
            new() { Id = Guid.NewGuid(), TipoMovimientoId = adelantoTypeId, Monto = 30000, Cantidad = 1, Fecha = inicio.AddDays(5) }
        };

        _uow.Movimientos.FindAsync(Arg.Any<Expression<Func<Movimiento, bool>>>())
            .Returns(adelantos);

        // Act
        var result = await _service.SugerirLiquidacionAsync(empleadoId, inicio, fin, 0);

        // Assert
        result.Should().NotBeNull();
        result.EmpleadoId.Should().Be(empleadoId);
        result.DiasTrabajados.Should().Be(11);
        result.TotalBruto.Should().Be(11 * tarifaDiaria); // 11 * 40000 = 440000
        result.TotalAdelantos.Should().Be(80000); // 50000 + 30000
        result.TotalNeto.Should().Be(360000); // 440000 - 80000
    }
    [Fact]
    public async Task GetAllAsync_ShouldReturnList()
    {
        // Arrange
        var list = new List<Liquidacion> { new() { Empleado = new Empleado { Nombre = "Juan" } } };
        _uow.Liquidaciones.GetAllWithEmpleadoAsync().Returns(list);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnDto_WhenSuccess()
    {
        // Arrange
        var liquidacion = new Liquidacion { Id = Guid.NewGuid() };
        _uow.SaveChangesAsync().Returns(1);

        // Act
        var result = await _service.CreateAsync(new ElectroObraApp.Application.DTOs.LiquidacionDto());

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task Sugerir_ShouldHandleNoAdelantos()
    {
        // Arrange
        var empleadoId = Guid.NewGuid();
        var empleado = new Empleado { Id = empleadoId, TarifaDiaria = 1000 };
        _uow.Repository<Empleado>().GetByIdAsync(empleadoId).Returns(empleado);
        _uow.Movimientos.FindAsync(Arg.Any<Expression<Func<Movimiento, bool>>>()).Returns(new List<Movimiento>());

        // Act
        var result = await _service.SugerirLiquidacionAsync(empleadoId, DateTime.Now, DateTime.Now, 1);

        // Assert
        result.TotalAdelantos.Should().Be(0);
        result.TotalNeto.Should().Be(1000);
    }
}

