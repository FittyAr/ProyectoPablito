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
    public async Task SaveCommand_ShouldCreate_WhenSuccess()
    {
        // Arrange
        _liquidacionService.CreateAsync(_viewModel.Liquidacion).Returns(new LiquidacionDto());
        bool closed = false;
        _viewModel.CloseRequest += (s, success) => closed = success;

        // Act
        await _viewModel.SaveCommand.ExecuteAsync(null);

        // Assert
        await _liquidacionService.Received(1).CreateAsync(Arg.Any<LiquidacionDto>());
        closed.Should().BeTrue();
    }

    [Fact]
    public async Task SugerirCommand_ShouldUpdateLiquidacion()
    {
        // Arrange
        var empleadoId = Guid.NewGuid();
        _viewModel.Liquidacion.EmpleadoId = empleadoId;
        var sugerencia = new LiquidacionDto { TotalNeto = 5000 };
        _liquidacionService.SugerirLiquidacionAsync(empleadoId, Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<decimal>())
            .Returns(sugerencia);

        // Act
        await _viewModel.SugerirCommand.ExecuteAsync(null);

        // Assert
        _viewModel.Liquidacion.TotalNeto.Should().Be(5000);
    }

    [Fact]
    public async Task LoadData_ShouldPopulateEmpleados()
    {
        // Arrange
        var emps = new List<EmpleadoDto> { new() { Nombre = "Emp 1" } };
        _empleadoService.GetAllAsync().Returns(emps);

        // Act
        await _viewModel.LoadDataCommand.ExecuteAsync(null);

        // Assert
        _viewModel.Empleados.Should().HaveCount(1);
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

    [Fact]
    public void FechaOffsets_ShouldUpdateDates()
    {
        // Arrange
        var newStart = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var newEnd = new DateTimeOffset(2026, 1, 15, 0, 0, 0, TimeSpan.Zero);

        // Act
        _viewModel.FechaInicioOffset = newStart;
        _viewModel.FechaFinOffset = newEnd;

        // Assert
        _viewModel.Liquidacion.FechaInicio.Should().Be(newStart.DateTime);
        _viewModel.Liquidacion.FechaFin.Should().Be(newEnd.DateTime);
    }
}

