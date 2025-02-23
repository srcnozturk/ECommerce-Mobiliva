using ECommerceAPI.Core.Entities;
using ECommerceAPI.Infrastructure;
using ECommerceAPI.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.UnitTests.Repositories;

public class ProductRepositoryTests : IDisposable
{
    private readonly DbContextOptions<ApplicationDbContext> _options;
    private readonly ApplicationDbContext _context;
    private readonly ProductRepository _repository;

    public ProductRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "ECommerceAPI-TestDb-" + Guid.NewGuid())
            .Options;

        _context = new ApplicationDbContext(_options);
        _repository = new ProductRepository(_context);

        SeedTestData();
    }

    private void SeedTestData()
    {
        var products = new List<ProductEntity>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Description = "Test Description 1",
                UnitPrice = 100,
                Category = "Kategori To",
                Unit = "Adet",
                Status = true,
                CreateDate = DateTime.UtcNow.AddDays(-1)
            },
            new()
            {
                Id = Guid.NewGuid(),
                Description = "Test Description 2",
                UnitPrice = 200,
                Category = "Kategori A",
                Unit = "Adet",
                Status = true,
                CreateDate = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                Description = "Test Description 3",
                UnitPrice = 300,
                Category = "Kategori B",
                Unit = "Kg",
                Status = false,
                CreateDate = DateTime.UtcNow
            }
        };

        _context.Products.AddRange(products);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnOnlyActiveProducts()
    {
        // Act
        var products = await _repository.GetAllAsync();

        // Assert
        products.Should().HaveCount(2);
        products.Should().OnlyContain(p => p.Status);
        products.Should().BeInDescendingOrder(p => p.CreateDate);
    }

    [Fact]
    public async Task GetByCategoryAsync_ShouldReturnOnlyActiveProductsInCategory()
    {
        // Arrange
        var category = "Kategori To";

        // Act
        var products = await _repository.GetByCategoryAsync(category);

        // Assert
        products.Should().HaveCount(1);
        products.Should().OnlyContain(p => p.Category == category && p.Status);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
