using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using ProyectoPablito.Application.DTOs;
using ProyectoPablito.Application.Interfaces;
using ProyectoPablito.ViewModels;
using Xunit;

namespace ProyectoPablito.Tests.UI.ViewModels;

public class LiquidacionesViewModelTests
{
    private readonly ILiquidacionService _liquidacionService;
    private readonly IExportService _exportService;
    private readonly IMovimientoService _movimientoService;
    private readonly LiquidacionesViewModel _viewModel;

    public LiquidacionesViewModelTests()
    {
        _liquidacionService = Substitute.For<ILiquidacionService>();
        _exportService = Substitute.For<IExportService>();
        _movimientoService = Substitute.For<IMovimientoService>();

        _viewModel = new LiquidacionesViewModel(_liquidacionService, _exportService, _movimientoService);
    }

    [Fact]
    public async Task LoadAsync_ShouldPopulateLiquidaciones()
    {
        // Arrange
        var list = new List<LiquidacionDto> { new() { EmpleadoNombre = "Pablo" } };
        _liquidacionService.GetAllAsync().Returns(list);

        // Act
        await _viewModel.LoadAsync();

        // Assert
        _viewModel.Liquidaciones.Should().HaveCount(1);
        _viewModel.Liquidaciones[0].EmpleadoNombre.Should().Be("Pablo");
        _viewModel.HasLiquidaciones.Should().BeTrue();
    }

    [Fact]
    public void NuevaLiquidacionCommand_ShouldInvokeAction()
    {
        // Arrange
        bool invoked = false;
        _viewModel.OnNuevaLiquidacion = () => invoked = true;

        // Act
        _viewModel.NuevaLiquidacionCommand.Execute(null);

        // Assert
        invoked.Should().BeTrue();
    }
}
