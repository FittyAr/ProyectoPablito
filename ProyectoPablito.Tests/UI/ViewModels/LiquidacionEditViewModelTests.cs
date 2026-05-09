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

public class LiquidacionEditViewModelTests
{
    private readonly ILiquidacionService _liquidacionService;
    private readonly IEmpleadoService _empleadoService;
    private readonly LiquidacionEditViewModel _viewModel;

    public LiquidacionEditViewModelTests()
    {
        _liquidacionService = Substitute.For<ILiquidacionService>();
        _empleadoService = Substitute.For<IEmpleadoService>();
        _viewModel = new LiquidacionEditViewModel(_liquidacionService, _empleadoService);
    }

    [Fact]
    public async Task SugerirAsync_ShouldUpdateLiquidacion()
    {
        // Arrange
        var id = Guid.NewGuid();
        _viewModel.Liquidacion.EmpleadoId = id;
        var sugerencia = new LiquidacionDto { EmpleadoId = id, TotalBruto = 500 };
        _liquidacionService.SugerirLiquidacionAsync(id, Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<decimal>())
            .Returns(sugerencia);

        // Act
        await _viewModel.SugerirCommand.ExecuteAsync(null);

        // Assert
        _viewModel.Liquidacion.TotalBruto.Should().Be(500);
    }

    [Fact]
    public async Task SaveCommand_ShouldInvokeCloseRequest_WhenSuccess()
    {
        // Arrange
        _liquidacionService.CreateAsync(Arg.Any<LiquidacionDto>()).Returns(new LiquidacionDto { Id = Guid.NewGuid() });
        bool closed = false;
        _viewModel.CloseRequest += (s, success) => closed = success;

        // Act
        await _viewModel.SaveCommand.ExecuteAsync(null);

        // Assert
        closed.Should().BeTrue();
    }
}
