using DemoApi.Configuration;
using DemoApi.Models;
using DemoApi.Services;
using DemoApi.Tests.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using MongoDB.Driver;

namespace DemoApi.Tests.Unit;

public class ProductServiceTests
{
    private readonly Mock<IMongoCollection<Product>> _mockCollection;
    private readonly Mock<MongoDbService> _mockMongoDbService;
    private readonly ProductService _sut;

    public ProductServiceTests()
    {
        _mockCollection = new Mock<IMongoCollection<Product>>();

        var settings = Options.Create(new MongoDbSettings
        {
            ConnectionString = "mongodb://localhost:27017",
            DatabaseName = "testdb",
            ProductsCollectionName = "products"
        });

        _mockMongoDbService = new Mock<MongoDbService>(settings);
        _mockMongoDbService
            .Setup(m => m.GetCollection<Product>("products"))
            .Returns(_mockCollection.Object);

        _sut = new ProductService(_mockMongoDbService.Object, settings);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllProducts()
    {
        // Arrange
        var expected = ProductFixtures.GetSampleProducts();
        var mockCursor = CreateMockCursor(expected);

        _mockCollection
            .Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<Product>>(),
                It.IsAny<FindOptions<Product, Product>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockCursor.Object);

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.Should().HaveCount(3);
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetByIdAsync_ValidId_ReturnsProduct()
    {
        // Arrange
        var expected = ProductFixtures.GetSingleProduct();
        var mockCursor = CreateMockCursor([expected]);

        _mockCollection
            .Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<Product>>(),
                It.IsAny<FindOptions<Product, Product>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockCursor.Object);

        // Act
        var result = await _sut.GetByIdAsync(expected.Id!);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(expected.Id);
        result.Name.Should().Be(expected.Name);
    }

    [Fact]
    public async Task GetByIdAsync_InvalidId_ReturnsNull()
    {
        // Arrange
        var mockCursor = CreateMockCursor([]);

        _mockCollection
            .Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<Product>>(),
                It.IsAny<FindOptions<Product, Product>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockCursor.Object);

        // Act
        var result = await _sut.GetByIdAsync("000000000000000000000000");

        // Assert
        result.Should().BeNull();
    }

    private static Mock<IAsyncCursor<Product>> CreateMockCursor(List<Product> products)
    {
        var mockCursor = new Mock<IAsyncCursor<Product>>();
        mockCursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                  .ReturnsAsync(true)
                  .ReturnsAsync(false);
        mockCursor.Setup(c => c.Current).Returns(products);
        return mockCursor;
    }
}