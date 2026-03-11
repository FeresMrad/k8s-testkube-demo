using FluentAssertions;

namespace DemoApp.E2E.Tests;

public class ReportPageTests : PageTestBase
{
    [Fact]
    public async Task ReportPage_DownloadButton_IsVisible()
    {
        // Act
        await Page.GotoAsync($"{BaseUrl}/report");

        // Assert
        var heading = Page.Locator("h2");
        await heading.WaitForAsync();
        var headingText = await heading.InnerTextAsync();
        headingText.Should().Contain("Report");

        var downloadBtn = Page.Locator("#download-report-btn");
        await downloadBtn.WaitForAsync();
        var isVisible = await downloadBtn.IsVisibleAsync();
        isVisible.Should().BeTrue();
    }
}