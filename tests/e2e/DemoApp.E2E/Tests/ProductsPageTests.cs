using FluentAssertions;
using Microsoft.Playwright;

namespace DemoApp.E2E.Tests;

public class ProductsPageTests : PageTestBase
{
    [Fact]
    public async Task ProductsPage_LoadsAndDisplaysTable()
    {
        // Act
        await Page.GotoAsync($"{BaseUrl}/products");

        // Assert — wait for the table to appear
        var table = Page.Locator("table");
        await table.WaitForAsync(new LocatorWaitForOptions { Timeout = 10_000 });

        var rows = Page.Locator("tr[mat-row]");
        var rowCount = await rows.CountAsync();
        rowCount.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task ProductsPage_ViewDetails_NavigatesToDetailPage()
    {
        // Arrange
        await Page.GotoAsync($"{BaseUrl}/products");

        var firstViewDetailsBtn = Page.Locator("button:has-text('View Details')").First;
        await firstViewDetailsBtn.WaitForAsync(new LocatorWaitForOptions { Timeout = 10_000 });

        // Act
        await firstViewDetailsBtn.ClickAsync();

        // Assert — wait for the detail page DOM, not navigation event
        var backButton = Page.Locator("button:has-text('Back to Products')");
        await backButton.WaitForAsync(new LocatorWaitForOptions { Timeout = 10_000 });

        Page.Url.Should().MatchRegex(@"/products/[a-zA-Z0-9]+$");
    }
}