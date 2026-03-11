using FluentAssertions;
using Microsoft.Playwright;

namespace DemoApp.E2E.Tests;

public class HomePageTests : PageTestBase
{
    [Fact]
    public async Task HomePage_LoadsSuccessfully()
    {
        // Act
        await Page.GotoAsync(BaseUrl);

        // Assert
        var title = await Page.TitleAsync();
        title.Should().Be("DemoUi");

        var heading = Page.Locator("h1");
        await heading.WaitForAsync();
        var headingText = await heading.InnerTextAsync();
        headingText.Should().Contain("K8s");
    }

    [Fact]
    public async Task HomePage_ViewProductsButton_NavigatesToProductsPage()
    {
        // Arrange
        await Page.GotoAsync(BaseUrl);

        // Act
        await Page.GetByRole(AriaRole.Button, new() { Name = "View Products" }).ClickAsync();

        // Assert — wait for the products table to appear, not navigation event
        var table = Page.Locator("table");
        await table.WaitForAsync(new LocatorWaitForOptions { Timeout = 10_000 });
        Page.Url.Should().Contain("/products");
    }

    [Fact]
    public async Task HomePage_GoToReportButton_NavigatesToReportPage()
    {
        // Arrange
        await Page.GotoAsync(BaseUrl);

        // Act
        await Page.GetByRole(AriaRole.Button, new() { Name = "Go to Report" }).ClickAsync();

        // Assert — wait for the download button to appear, not navigation event
        var downloadBtn = Page.Locator("#download-report-btn");
        await downloadBtn.WaitForAsync(new LocatorWaitForOptions { Timeout = 10_000 });
        Page.Url.Should().Contain("/report");
    }
}