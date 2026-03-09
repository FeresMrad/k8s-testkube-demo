using DemoApi.Services;
using DemoApi.Tests.Fixtures;
using FluentAssertions;
using QuestPDF.Infrastructure;

namespace DemoApi.Tests.Unit;

public class PdfServiceTests
{
    private readonly PdfService _sut;

    public PdfServiceTests()
    {
        // Required for QuestPDF in test context
        QuestPDF.Settings.License = LicenseType.Community;
        _sut = new PdfService();
    }

    [Fact]
    public void GenerateProductReport_ReturnsNonEmptyByteArray()
    {
        // Arrange
        var products = ProductFixtures.GetSampleProducts();

        // Act
        var result = _sut.GenerateProductReport(products);

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
    }

    [Fact]
    public void GenerateProductReport_EmptyList_StillGeneratesPdf()
    {
        // Act
        var result = _sut.GenerateProductReport([]);

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
    }

    [Fact]
    public void GenerateProductReport_ReturnsPdfWithCorrectHeader()
    {
        // Arrange
        var products = ProductFixtures.GetSampleProducts();

        // Act
        var result = _sut.GenerateProductReport(products);

        // Assert — PDF files always start with the magic bytes "%PDF"
        var header = System.Text.Encoding.ASCII.GetString(result[..4]);
        header.Should().Be("%PDF");
    }
}