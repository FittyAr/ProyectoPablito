using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using ProyectoPablito.Application.DTOs;
using ProyectoPablito.Application.Services;
using ProyectoPablito.Core.Entities;
using ProyectoPablito.Core.Interfaces;
using Xunit;

namespace ProyectoPablito.Tests.Application.Services;

public class EmpleadoServiceTests
{
    private readonly IUnitOfWork _uow;
    private readonly IRepository<Empleado> _repo;
    private readonly ILogger<EmpleadoService> _logger;
    private readonly EmpleadoService _service;

    public EmpleadoServiceTests()
    {
        _uow = Substitute.For<IUnitOfWork>();
        _repo = Substitute.For<IRepository<Empleado>>();
        _uow.Repository<Empleado>().Returns(_repo);
        _logger = Substitute.For<ILogger<EmpleadoService>>();
        _service = new EmpleadoService(_uow, _logger);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnList()
    {
        // Arrange
        var list = new List<Empleado> { new() { Nombre = "Pablo" } };
        _repo.GetAllAsync().Returns(list);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().HaveCount(1);
        result.First().Nombre.Should().Be("Pablo");
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnTrue_WhenSuccess()
    {
        // Arrange
        var dto = new EmpleadoDto { Nombre = "Nuevo" };
        _uow.SaveChangesAsync().Returns(1);

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        result.Should().BeTrue();
        await _repo.Received(1).AddAsync(Arg.Any<Empleado>());
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnTrue_WhenSuccess()
    {
        // Arrange
        var dto = new EmpleadoDto { Id = Guid.NewGuid(), Nombre = "Update" };
        _uow.SaveChangesAsync().Returns(1);

        // Act
        var result = await _service.UpdateAsync(dto);

        // Assert
        result.Should().BeTrue();
        _repo.Received(1).Update(Arg.Any<Empleado>());
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repo.GetByIdAsync(id).Returns((Empleado?)null);

        // Act
        var result = await _service.DeleteAsync(id);

        // Assert
        result.Should().BeFalse();
        _repo.DidNotReceive().Remove(Arg.Any<Empleado>());
    }
}
