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
    private readonly IUserSettingsService _settingsService;
    private readonly LiquidacionEditViewModel _viewModel;

    public LiquidacionEditViewModelTests()
    {
        _liquidacionService = Substitute.For<ILiquidacionService>();
        _empleadoService = Substitute.For<IEmpleadoService>();
        _settingsService = Substitute.For<IUserSettingsService>();
        
        _settingsService.GetDefaultMultiplierSaturday().Returns(1.0m);
        _settingsService.GetDefaultMultiplierSunday().Returns(1.0m);
        _settingsService.GetDefaultMultiplierHoliday().Returns(1.0m);

        _viewModel = new LiquidacionEditViewModel(_liquidacionService, _empleadoService, _settingsService);
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
        // Lunes 2026-05-04 a Viernes 2026-05-08 (5 días hábiles)
        _viewModel.Liquidacion.FechaInicio = new DateTime(2026, 5, 4);
        _viewModel.Liquidacion.FechaFin = new DateTime(2026, 5, 8);
        
        var sugerencia = new LiquidacionDto { TarifaAplicada = 1000, TotalAdelantos = 500 };
        _liquidacionService.SugerirLiquidacionAsync(empleadoId, Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<decimal>())
            .Returns(sugerencia);

        // Act
        await _viewModel.SugerirCommand.ExecuteAsync(null);

        // Assert
        // 5 días * 1000 = 5000 bruto. Neto = 5000 - 500 = 4500
        _viewModel.Liquidacion.TotalBruto.Should().Be(5000);
        _viewModel.Liquidacion.TotalNeto.Should().Be(4500);
        _viewModel.Liquidacion.DiasTrabajados.Should().Be(5);
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

