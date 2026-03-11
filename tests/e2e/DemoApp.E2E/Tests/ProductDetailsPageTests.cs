using FluentAssertions;
using Microsoft.Playwright;

namespace DemoApp.E2E.Tests;

public class ProductDetailsPageTests : PageTestBase
{
    [Fact]
    public async Task ProductDetailsPage_DisplaysProductInfo()
    {
        // Arrange — navigate via products page to get a real ID
        await Page.GotoAsync($"{BaseUrl}/products");

        var firstViewDetailsBtn = Page.Locator("button:has-text('View Details')").First;
        await firstViewDetailsBtn.WaitForAsync(new LocatorWaitForOptions
        {
            Timeout = 10_000
        });

        // Act
        await firstViewDetailsBtn.ClickAsync();

        // Assert — wait for the detail page DOM, not navigation event
        var backButton = Page.Locator("button:has-text('Back to Products')");
        await backButton.WaitForAsync(new LocatorWaitForOptions { Timeout = 10_000 });

        Page.Url.Should().MatchRegex(@"/products/[a-zA-Z0-9]+$");

        var isVisible = await backButton.IsVisibleAsync();
        isVisible.Should().BeTrue();

        // Card content is present
        var card = Page.Locator("mat-card");
        await card.WaitForAsync(new LocatorWaitForOptions { Timeout = 10_000 });
        var cardText = await card.InnerTextAsync();
        cardText.Should().NotBeNullOrWhiteSpace();

        // Author and Published fields are present
        var pageContent = await Page.ContentAsync();
        pageContent.Should().Contain("Author:");
        pageContent.Should().Contain("Published:");
    }
}