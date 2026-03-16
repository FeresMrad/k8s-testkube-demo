using Microsoft.Playwright;

namespace DemoApp.E2E;

public class PageTestBase : IAsyncLifetime
{
    protected IPlaywright Playwright { get; private set; } = null!;
    protected IBrowser Browser { get; private set; } = null!;
    protected IBrowserContext Context { get; private set; } = null!;
    protected IPage Page { get; private set; } = null!;

    protected virtual string BaseUrl => "http://demo.local";

    public async Task InitializeAsync()
    {
        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();

        Browser = await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });

        // Video is recorded per browser context.
        // This path is relative to the test project's working directory.
        Context = await Browser.NewContextAsync(new BrowserNewContextOptions
        {
            RecordVideoDir = "TestResults/videos",
            RecordVideoSize = new RecordVideoSize { Width = 1280, Height = 720 }
        });

        Page = await Context.NewPageAsync();
    }

    public async Task DisposeAsync()
    {
        await Page.CloseAsync();

        // Important: closing the context finalizes (flushes) the video file to disk.
        await Context.CloseAsync();

        await Browser.CloseAsync();
        Playwright.Dispose();
    }
}