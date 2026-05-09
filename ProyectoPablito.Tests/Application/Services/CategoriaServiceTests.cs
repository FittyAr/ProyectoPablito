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
}
