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
        await Page.WaitForURLAsync(new Regex($"{BaseUrl}/products/.+"));

        // Assert — key elements are visible on the details page
        var backButton = Page.Locator("button:has-text('Back to Products')");
        await backButton.WaitForAsync();
        await backButton.IsVisibleAsync();

        // Card content is present
        var card = Page.Locator("mat-card");
        await card.WaitForAsync();
        var cardText = await card.InnerTextAsync();
        cardText.Should().NotBeNullOrWhiteSpace();

        // Author and Published fields are present
        var pageContent = await Page.ContentAsync();
        pageContent.Should().Contain("Author:");
        pageContent.Should().Contain("Published:");
    }
}