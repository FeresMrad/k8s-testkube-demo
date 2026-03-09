using System.Net;
using System.Text.Json;
using DemoApi.Models;
using DemoApi.Services;
using DemoApi.Tests.Fixtures;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace DemoApi.Tests.Integration;

public class ProductsEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly List<Product> _sampleProducts;

    public ProductsEndpointTests(WebApplicationFactory<Program> factory)
    {
        _sampleProducts = ProductFixtures.GetSampleProducts();

        var mockProductService = new Mock<ProductService>();
        mockProductService
            .Setup(s => s.GetAllAsync())
            .ReturnsAsync(_sampleProducts);
        mockProductService
            .Setup(s => s.GetByIdAsync(_sampleProducts[0].Id!))
            .ReturnsAsync(_sampleProducts[0]);
        mockProductService
            .Setup(s => s.GetByIdAsync("000000000000000000000000"))
            .ReturnsAsync((Product?)null);

        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove the real ProductService and replace with mock
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(ProductService));
                if (descriptor != null)
                    services.Remove(descriptor);

                services.AddScoped(_ => mockProductService.Object);
            });
        }).CreateClient();
    }

    [Fact]
    public async Task GetProducts_Returns200WithList()
    {
        // Act
        var response = await _client.GetAsync("/api/products");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadAsStringAsync();
        var products = JsonSerializer.Deserialize<List<Product>>(body,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        products.Should().NotBeNull();
        products!.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetProductById_ValidId_Returns200WithProduct()
    {
        // Arrange
        var id = _sampleProducts[0].Id!;

        // Act
        var response = await _client.GetAsync($"/api/products/{id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadAsStringAsync();
        var product = JsonSerializer.Deserialize<Product>(body,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        product.Should().NotBeNull();
        product!.Id.Should().Be(id);
        product.Name.Should().Be(_sampleProducts[0].Name);
    }

    [Fact]
    public async Task GetProductById_InvalidId_Returns404()
    {
        // Act
        var response = await _client.GetAsync("/api/products/000000000000000000000000");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}