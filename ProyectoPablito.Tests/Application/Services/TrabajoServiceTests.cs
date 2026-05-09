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

public class TrabajoServiceTests
{
    private readonly IUnitOfWork _uow;
    private readonly IRepository<Trabajo> _repo;
    private readonly ITrabajoRepository _trabajoRepo;
    private readonly ILogger<TrabajoService> _logger;
    private readonly TrabajoService _service;

    public TrabajoServiceTests()
    {
        _uow = Substitute.For<IUnitOfWork>();
        _repo = Substitute.For<IRepository<Trabajo>>();
        _trabajoRepo = Substitute.For<ITrabajoRepository>();
        _uow.Repository<Trabajo>().Returns(_repo);
        _uow.Trabajos.Returns(_trabajoRepo);
        _logger = Substitute.For<ILogger<TrabajoService>>();
        _service = new TrabajoService(_uow, _logger);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnList()
    {
        // Arrange
        var list = new List<Trabajo> { new() { Descripcion = "Trabajo 1" } };
        _trabajoRepo.GetAllWithClienteAsync().Returns(list);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().HaveCount(1);
        result.First().Descripcion.Should().Be("Trabajo 1");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnDto_WhenFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity = new Trabajo { Id = id, Descripcion = "Test" };
        _trabajoRepo.GetByIdWithOrdenesAsync(id).Returns(entity);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        result.Should().NotBeNull();
        result!.Descripcion.Should().Be("Test");
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnTrue_WhenSuccess()
    {
        // Arrange
        var dto = new TrabajoDto { Descripcion = "Nuevo" };
        _uow.SaveChangesAsync().Returns(1);

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        result.Should().BeTrue();
        await _repo.Received(1).AddAsync(Arg.Any<Trabajo>());
    }
}
