using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using ElectroObraApp.Application.DTOs;
using ElectroObraApp.Application.Interfaces;
using ElectroObraApp.ViewModels;
using Xunit;

namespace ElectroObraApp.Tests.UI.ViewModels;

public class DashboardViewModelTests
{
    private readonly IMovimientoService _movimientoService;
    private readonly IClienteService _clienteService;
    private readonly ITrabajoService _trabajoService;
    private readonly IUserSettingsService _settingsService;
    private readonly IDollarService _dollarService;

    public DashboardViewModelTests()
    {
        _movimientoService = Substitute.For<IMovimientoService>();
        _clienteService = Substitute.For<IClienteService>();
        _trabajoService = Substitute.For<ITrabajoService>();
        _settingsService = Substitute.For<IUserSettingsService>();
        _dollarService = Substitute.For<IDollarService>();
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
        var vm = new DashboardViewModel(_movimientoService, _clienteService, _trabajoService, _settingsService, _dollarService);
        await vm.LoadStatsCommand.ExecuteAsync(null);

        // Assert
        vm.TotalIngresos.Should().Be(100);
        vm.TotalGastos.Should().Be(40);
        vm.Balance.Should().Be(60);
    }

    [Fact]
    public async Task LoadStats_ShouldHandleZeroMovements()
    {
        // Arrange
        _movimientoService.GetAllAsync().Returns(new List<MovimientoDto>());

        // Act
        var vm = new DashboardViewModel(_movimientoService, _clienteService, _trabajoService, _settingsService, _dollarService);
        await vm.LoadStatsCommand.ExecuteAsync(null);

        // Assert
        vm.TotalIngresos.Should().Be(0);
        vm.TotalGastos.Should().Be(0);
        vm.Balance.Should().Be(0);
    }

    [Fact]
    public async Task LoadStats_ShouldHandleOnlyIncome()
    {
        // Arrange
        var movimientos = new List<MovimientoDto>
        {
            new() { Total = 200, TipoMovimientoSuma = true }
        };
        _movimientoService.GetAllAsync().Returns(movimientos);

        // Act
        var vm = new DashboardViewModel(_movimientoService, _clienteService, _trabajoService, _settingsService, _dollarService);
        await vm.LoadStatsCommand.ExecuteAsync(null);

        // Assert
        vm.TotalIngresos.Should().Be(200);
        vm.TotalGastos.Should().Be(0);
        vm.Balance.Should().Be(200);
    }
}

