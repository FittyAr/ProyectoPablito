using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using ElectroObraApp.Application.DTOs;
using ElectroObraApp.Application.Interfaces;
using ElectroObraApp.ViewModels;
using Xunit;

namespace ElectroObraApp.Tests.UI.ViewModels;

public class MovimientoEditViewModelTests
{
    private readonly IMovimientoService _movimientoService;
    private readonly ICategoriaService _categoriaService;
    private readonly ITipoMovimientoService _tipoMovimientoService;
    private readonly IEmpleadoService _empleadoService;
    private readonly MovimientoEditViewModel _viewModel;

    public MovimientoEditViewModelTests()
    {
        _movimientoService = Substitute.For<IMovimientoService>();
        _categoriaService = Substitute.For<ICategoriaService>();
        _tipoMovimientoService = Substitute.For<ITipoMovimientoService>();
        _empleadoService = Substitute.For<IEmpleadoService>();
        _viewModel = new MovimientoEditViewModel(_movimientoService, _categoriaService, _tipoMovimientoService, _empleadoService);
    }

    [Fact]
    public async Task SaveCommand_ShouldCreate_WhenIdIsEmpty()
    {
        // Arrange
        _viewModel.Movimiento.Monto = 100;
        _movimientoService.CreateAsync(_viewModel.Movimiento).Returns(true);
        bool closed = false;
        _viewModel.CloseRequest += (s, success) => closed = success;

        // Act
        await _viewModel.SaveCommand.ExecuteAsync(null);

        // Assert
        await _movimientoService.Received(1).CreateAsync(Arg.Any<MovimientoDto>());
        closed.Should().BeTrue();
    }

    [Fact]
    public async Task SaveCommand_ShouldUpdate_WhenIdIsNotEmpty()
    {
        // Arrange
        _viewModel.Movimiento = new MovimientoDto { Id = Guid.NewGuid(), Monto = 200 };
        _movimientoService.UpdateAsync(_viewModel.Movimiento).Returns(true);
        bool closed = false;
        _viewModel.CloseRequest += (s, success) => closed = success;

        // Act
        await _viewModel.SaveCommand.ExecuteAsync(null);

        // Assert
        await _movimientoService.Received(1).UpdateAsync(Arg.Any<MovimientoDto>());
        closed.Should().BeTrue();
    }

    [Fact]
    public async Task LoadData_ShouldPopulateCollections()
    {
        // Arrange
        var cats = new List<CategoriaDto> { new() { Nombre = "Cat 1" } };
        var tipos = new List<TipoMovimientoDto> { new() { Nombre = "Tipo 1" } };
        _categoriaService.GetAllAsync().Returns(cats);
        _tipoMovimientoService.GetAllAsync().Returns(tipos);
        _empleadoService.GetAllAsync().Returns(new List<EmpleadoDto>());

        // Act
        await _viewModel.LoadDataCommand.ExecuteAsync(null);

        // Assert
        _viewModel.Categorias.Should().HaveCount(1);
        _viewModel.TiposMovimiento.Should().HaveCount(1);
    }

    [Fact]
    public async Task LoadData_ShouldSetDefaultTipo_WhenNew()
    {
        // Arrange
        var tipoId = Guid.NewGuid();
        var tipos = new List<TipoMovimientoDto> { new() { Id = tipoId, Nombre = "Tipo 1" } };
        _tipoMovimientoService.GetAllAsync().Returns(tipos);
        _categoriaService.GetAllAsync().Returns(new List<CategoriaDto>());
        _empleadoService.GetAllAsync().Returns(new List<EmpleadoDto>());

        // Act
        await _viewModel.LoadDataCommand.ExecuteAsync(null);

        // Assert
        _viewModel.Movimiento.TipoMovimientoId.Should().Be(tipoId);
    }

    [Fact]
    public void CancelCommand_ShouldInvokeCloseRequestWithFalse()
    {
        // Arrange
        bool closedWithSuccess = true;
        _viewModel.CloseRequest += (s, success) => closedWithSuccess = success;

        // Act
        _viewModel.CancelCommand.Execute(null);

        // Assert
        closedWithSuccess.Should().BeFalse();
    }
}

