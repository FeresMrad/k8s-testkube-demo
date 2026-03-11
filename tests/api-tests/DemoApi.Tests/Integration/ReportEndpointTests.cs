using System.Net;
using DemoApi.Services;
using DemoApi.Tests.Fixtures;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace DemoApi.Tests.Integration;

public class ReportEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ReportEndpointTests(WebApplicationFactory<Program> factory)
    {
        var sampleProducts = ProductFixtures.GetSampleProducts();

        // Mock the interface — no constructor issues
        var mockProductService = new Mock<IProductService>();
        mockProductService
            .Setup(s => s.GetAllAsync())
            .ReturnsAsync(sampleProducts);

        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IProductService));
                if (descriptor != null)
                    services.Remove(descriptor);

                services.AddScoped(_ => mockProductService.Object);
            });
        }).CreateClient();
    }

    [Fact]
    public async Task GetReport_Returns200WithPdfContentType()
    {
        // Act
        var response = await _client.GetAsync("/api/report/download");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/pdf");
    }

    [Fact]
    public async Task GetReport_ReturnedBytesArePdf()
    {
        // Act
        var response = await _client.GetAsync("/api/report/download");
        var bytes = await response.Content.ReadAsByteArrayAsync();

        // Assert
        bytes.Should().NotBeEmpty();
        var header = System.Text.Encoding.ASCII.GetString(bytes[..4]);
        header.Should().Be("%PDF");
    }
}