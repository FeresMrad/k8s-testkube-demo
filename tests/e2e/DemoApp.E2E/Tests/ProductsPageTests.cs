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
        await table.WaitForAsync(new LocatorWaitForOptions
        {
            Timeout = 10_000
        });

        var rows = Page.Locator("tr[mat-row]");
        var rowCount = await rows.CountAsync();
        rowCount.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task ProductsPage_ViewDetails_NavigatesToDetailPage()
    {
        // Arrange
        await Page.GotoAsync($"{BaseUrl}/products");

        // Wait for the table rows to load
        var firstViewDetailsBtn = Page.Locator("button:has-text('View Details')").First;
        await firstViewDetailsBtn.WaitForAsync(new LocatorWaitForOptions
        {
            Timeout = 10_000
        });

        // Act
        await firstViewDetailsBtn.ClickAsync();

        // Assert — URL should be /products/{some-id}
        await Page.WaitForURLAsync(new Regex($"{BaseUrl}/products/.+"));
        Page.Url.Should().MatchRegex(@"/products/[a-zA-Z0-9]+$");
    }
}