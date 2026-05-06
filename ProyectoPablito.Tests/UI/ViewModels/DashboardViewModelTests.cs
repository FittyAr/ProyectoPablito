using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using ProyectoPablito.Application.DTOs;
using ProyectoPablito.Application.Interfaces;
using ProyectoPablito.ViewModels;
using Xunit;

namespace ProyectoPablito.Tests.UI.ViewModels;

public class DashboardViewModelTests
{
    private readonly IMovimientoService _movimientoService;

    public DashboardViewModelTests()
    {
        _movimientoService = Substitute.For<IMovimientoService>();
    }

    [Fact]
    public async Task Constructor_ShouldCalculateTotals()
    {
        // Arrange
        var movimientos = new List<MovimientoDto>
        {
            new() { Monto = 100, Cantidad = 1, Total = 100, TipoMovimientoSuma = true },
            new() { Monto = 40, Cantidad = 1, Total = 40, TipoMovimientoSuma = false }
        };
        _movimientoService.GetAllAsync().Returns(movimientos);

        // Act
        var vm = new DashboardViewModel(_movimientoService);
        await vm.LoadStatsCommand.ExecuteAsync(null);

        // Assert
        vm.TotalIngresos.Should().Be(100);
        vm.TotalGastos.Should().Be(40);
        vm.Balance.Should().Be(60);
    }
}
