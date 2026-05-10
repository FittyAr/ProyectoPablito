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

public class MovimientosViewModelTests
{
    private readonly IMovimientoService _movimientoService;
    private readonly ITipoMovimientoService _tipoMovimientoService;
    private readonly IExportService _exportService;
    private readonly IUserSettingsService _settingsService;
    private readonly IServiceProvider _serviceProvider;
    private readonly MovimientosViewModel _vm;

    public MovimientosViewModelTests()
    {
        _movimientoService = Substitute.For<IMovimientoService>();
        _tipoMovimientoService = Substitute.For<ITipoMovimientoService>();
        _exportService = Substitute.For<IExportService>();
        _settingsService = Substitute.For<IUserSettingsService>();
        _serviceProvider = Substitute.For<IServiceProvider>();
        
        _settingsService.GetPageSize().Returns(10);
        
        _vm = new MovimientosViewModel(_movimientoService, _tipoMovimientoService, _exportService, _settingsService, _serviceProvider);
    }

    [Fact]
    public async Task LoadMovimientosCommand_ShouldPopulateMovimientos()
    {
        // Arrange
        var list = new List<MovimientoDto> { new() { Concepto = "Test" } };
        _movimientoService.GetAllAsync().Returns(list);

        // Act
        await _vm.LoadMovimientosCommand.ExecuteAsync(null);

        // Assert
        _vm.Movimientos.Should().HaveCount(1);
        _vm.Movimientos.First().Concepto.Should().Be("Test");
    }

    [Fact]
    public async Task FiltroConcepto_ShouldFilterList()
    {
        // Arrange
        var list = new List<MovimientoDto> 
        { 
            new() { Concepto = "Agua" },
            new() { Concepto = "Luz" }
        };
        _movimientoService.GetAllAsync().Returns(list);
        await _vm.LoadMovimientosCommand.ExecuteAsync(null);

        // Act
        _vm.FiltroConcepto = "Agua";
        // OnFiltroConceptoChanged calls LoadMovimientosAsync automatically

        // Assert
        _vm.Movimientos.Should().HaveCount(1);
        _vm.Movimientos.First().Concepto.Should().Be("Agua");
    }
}

