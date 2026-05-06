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

public class MovimientosViewModelTests
{
    private readonly IMovimientoService _movimientoService;
    private readonly IExportService _exportService;
    private readonly IServiceProvider _serviceProvider;
    private readonly MovimientosViewModel _vm;

    public MovimientosViewModelTests()
    {
        _movimientoService = Substitute.For<IMovimientoService>();
        _exportService = Substitute.For<IExportService>();
        _serviceProvider = Substitute.For<IServiceProvider>();
        _vm = new MovimientosViewModel(_movimientoService, _exportService, _serviceProvider);
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
}
