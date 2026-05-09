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

public class MovimientoEditViewModelTests
{
    private readonly IMovimientoService _movimientoService;
    private readonly ICategoriaService _categoriaService;
    private readonly ITipoMovimientoService _tipoMovimientoService;
    private readonly IEmpleadoService _empleadoService;
    private readonly MovimientoEditViewModel _vm;

    public MovimientoEditViewModelTests()
    {
        _movimientoService = Substitute.For<IMovimientoService>();
        _categoriaService = Substitute.For<ICategoriaService>();
        _tipoMovimientoService = Substitute.For<ITipoMovimientoService>();
        _empleadoService = Substitute.For<IEmpleadoService>();
        
        _vm = new MovimientoEditViewModel(_movimientoService, _categoriaService, _tipoMovimientoService, _empleadoService);
    }

    [Fact]
    public async Task LoadDataCommand_ShouldPopulateLists()
    {
        // Arrange
        var cats = new List<CategoriaDto> { new() { Nombre = "Cat1" } };
        var tipos = new List<TipoMovimientoDto> { new() { Nombre = "Tipo1" } };
        
        _categoriaService.GetAllAsync().Returns(cats);
        _tipoMovimientoService.GetAllAsync().Returns(tipos);

        // Act
        await _vm.LoadDataCommand.ExecuteAsync(null);

        // Assert
        _vm.Categorias.Should().HaveCount(1);
        _vm.TiposMovimiento.Should().HaveCount(1);
    }

    [Fact]
    public async Task SaveCommand_ShouldCallCreate_WhenNew()
    {
        // Arrange
        _vm.Movimiento = new MovimientoDto { Concepto = "Test", Monto = 100 };
        _movimientoService.CreateAsync(Arg.Any<MovimientoDto>()).Returns(true);

        // Act
        await _vm.SaveCommand.ExecuteAsync(null);

        // Assert
        await _movimientoService.Received(1).CreateAsync(Arg.Any<MovimientoDto>());
    }

    [Fact]
    public async Task SaveCommand_ShouldCallUpdate_WhenExisting()
    {
        // Arrange
        _vm.Movimiento = new MovimientoDto { Id = Guid.NewGuid(), Concepto = "Update", Monto = 200 };
        _movimientoService.UpdateAsync(Arg.Any<MovimientoDto>()).Returns(true);

        // Act
        await _vm.SaveCommand.ExecuteAsync(null);

        // Assert
        await _movimientoService.Received(1).UpdateAsync(Arg.Any<MovimientoDto>());
    }
}
