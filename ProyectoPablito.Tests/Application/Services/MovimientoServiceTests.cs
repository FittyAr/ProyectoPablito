using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using ProyectoPablito.Application.DTOs;
using ProyectoPablito.Application.Services;
using ProyectoPablito.Core.Entities;
using ProyectoPablito.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Xunit;

namespace ProyectoPablito.Tests.Application.Services;

public class MovimientoServiceTests
{
    private readonly IUnitOfWork _uow;
    private readonly IRepository<Movimiento> _repo;
    private readonly IMovimientoRepository _movimientoRepo;
    private readonly ILogger<MovimientoService> _logger;
    private readonly MovimientoService _service;

    public MovimientoServiceTests()
    {
        _uow = Substitute.For<IUnitOfWork>();
        _repo = Substitute.For<IRepository<Movimiento>>();
        _movimientoRepo = Substitute.For<IMovimientoRepository>();
        _uow.Repository<Movimiento>().Returns(_repo);
        _uow.Movimientos.Returns(_movimientoRepo);
        _logger = Substitute.For<ILogger<MovimientoService>>();
        _service = new MovimientoService(_uow, _logger);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnList()
    {
        // Arrange
        var list = new List<Movimiento> { new() { Concepto = "Sueldo" } };
        _movimientoRepo.GetAllWithIncludesAsync().Returns(list);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().HaveCount(1);
        result.First().Concepto.Should().Be("Sueldo");
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnTrue_WhenSaveIsSuccessful()
    {
        // Arrange
        var dto = new MovimientoDto { Concepto = "Test", Monto = 100, CategoriaId = Guid.NewGuid() };
        _uow.SaveChangesAsync().Returns(1);

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        result.Should().BeTrue();
        await _repo.Received(1).AddAsync(Arg.Any<Movimiento>());
        await _uow.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenEntityDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repo.GetByIdAsync(id).Returns((Movimiento?)null);

        // Act
        var result = await _service.DeleteAsync(id);

        // Assert
        result.Should().BeFalse();
        _repo.DidNotReceive().Remove(Arg.Any<Movimiento>());
    }
}
