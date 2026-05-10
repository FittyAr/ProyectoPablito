using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using ElectroObraApp.Application.DTOs;
using ElectroObraApp.Application.Services;
using ElectroObraApp.Core.Entities;
using ElectroObraApp.Core.Interfaces;
using Xunit;

namespace ElectroObraApp.Tests.Application.Services;

public class TipoMovimientoServiceTests
{
    private readonly IUnitOfWork _uow;
    private readonly IRepository<TipoMovimiento> _repo;
    private readonly TipoMovimientoService _service;

    public TipoMovimientoServiceTests()
    {
        _uow = Substitute.For<IUnitOfWork>();
        _repo = Substitute.For<IRepository<TipoMovimiento>>();
        _uow.Repository<TipoMovimiento>().Returns(_repo);
        _service = new TipoMovimientoService(_uow);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnList()
    {
        // Arrange
        var list = new List<TipoMovimiento> { new() { Nombre = "Efectivo" } };
        _repo.GetAllAsync().Returns(list);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().HaveCount(1);
        result.First().Nombre.Should().Be("Efectivo");
    }
}

