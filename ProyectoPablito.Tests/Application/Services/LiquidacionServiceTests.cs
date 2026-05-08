using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using ProyectoPablito.Application.Interfaces;
using ProyectoPablito.Application.Services;
using ProyectoPablito.Core.Entities;
using ProyectoPablito.Core.Interfaces;
using Xunit;

namespace ProyectoPablito.Tests.Application.Services;

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
    public async Task SugerirLiquidacionAsync_ShouldCalculateCorrectTotals()
    {
        // Arrange
        var empleadoId = Guid.NewGuid();
        var inicio = new DateTime(2026, 5, 1);
        var fin = new DateTime(2026, 5, 15);
        var diasTrabajados = 12m;
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
        var result = await _service.SugerirLiquidacionAsync(empleadoId, inicio, fin, diasTrabajados);

        // Assert
        result.Should().NotBeNull();
        result.EmpleadoId.Should().Be(empleadoId);
        result.TotalBruto.Should().Be(diasTrabajados * tarifaDiaria); // 12 * 40000 = 480000
        result.TotalAdelantos.Should().Be(80000); // 50000 + 30000
        result.TotalNeto.Should().Be(400000); // 480000 - 80000
    }
}
