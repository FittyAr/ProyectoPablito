using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using ElectroObraApp.Application.DTOs;
using ElectroObraApp.Application.Services;
using ElectroObraApp.Core.Entities;
using ElectroObraApp.Core.Interfaces;
using Xunit;

namespace ElectroObraApp.Tests.Application.Services;

public class CategoriaServiceTests
{
    private readonly IUnitOfWork _uow;
    private readonly IRepository<Categoria> _repo;
    private readonly ILogger<CategoriaService> _logger;
    private readonly CategoriaService _service;

    public CategoriaServiceTests()
    {
        _uow = Substitute.For<IUnitOfWork>();
        _repo = Substitute.For<IRepository<Categoria>>();
        _uow.Repository<Categoria>().Returns(_repo);
        _logger = Substitute.For<ILogger<CategoriaService>>();
        _service = new CategoriaService(_uow, _logger);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnList()
    {
        // Arrange
        var list = new List<Categoria> { new() { Nombre = "Ferretería" } };
        _repo.GetAllAsync().Returns(list);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().HaveCount(1);
        result.First().Nombre.Should().Be("Ferretería");
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnTrue_WhenSuccess()
    {
        // Arrange
        var dto = new CategoriaDto { Nombre = "Nueva" };
        _uow.SaveChangesAsync().Returns(1);

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        result.Should().BeTrue();
        await _repo.Received(1).AddAsync(Arg.Any<Categoria>());
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnTrue_WhenSuccess()
    {
        // Arrange
        var dto = new CategoriaDto { Id = Guid.NewGuid(), Nombre = "Update" };
        _repo.GetByIdAsync(dto.Id).Returns(new Categoria { Id = dto.Id });
        _uow.SaveChangesAsync().Returns(1);

        // Act
        var result = await _service.UpdateAsync(dto);

        // Assert
        result.Should().BeTrue();
        _repo.Received(1).Update(Arg.Any<Categoria>());
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnFalse_WhenNotFound()
    {
        // Arrange
        var dto = new CategoriaDto { Id = Guid.NewGuid(), Nombre = "Update" };
        _repo.GetByIdAsync(dto.Id).Returns((Categoria)null!);

        // Act
        var result = await _service.UpdateAsync(dto);

        // Assert
        result.Should().BeFalse();
        _repo.DidNotReceive().Update(Arg.Any<Categoria>());
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnTrue_WhenSuccess()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity = new Categoria { Id = id };
        _repo.GetByIdAsync(id).Returns(entity);
        _uow.SaveChangesAsync().Returns(1);

        // Act
        var result = await _service.DeleteAsync(id);

        // Assert
        result.Should().BeTrue();
        _repo.Received(1).Remove(entity);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repo.GetByIdAsync(id).Returns((Categoria)null!);

        // Act
        var result = await _service.DeleteAsync(id);

        // Assert
        result.Should().BeFalse();
        _repo.DidNotReceive().Remove(Arg.Any<Categoria>());
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnDto_WhenFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity = new Categoria { Id = id, Nombre = "Test" };
        _repo.GetByIdAsync(id).Returns(entity);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        result.Should().NotBeNull();
        result!.Nombre.Should().Be("Test");
    }
}

