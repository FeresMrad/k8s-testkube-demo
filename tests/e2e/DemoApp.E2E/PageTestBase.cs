using System;
using System.IO;
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

        // Compute an absolute path for TestResults/videos relative to where the test runner runs.
        // AppContext.BaseDirectory is the test runtime folder (e.g. bin/Debug/net10.0).
        // Climb back up to the project folder and place videos in ./TestResults/videos so the path
        // is stable whether running locally or in TestKube.
        var videoDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "TestResults", "videos"));
        Directory.CreateDirectory(videoDir);

        Context = await Browser.NewContextAsync(new BrowserNewContextOptions
        {
            RecordVideoDir = videoDir,
            RecordVideoSize = new RecordVideoSize { Width = 1280, Height = 720 }
        });

        Page = await Context.NewPageAsync();
    }

    public async Task DisposeAsync()
    {
        await Page.CloseAsync();

        // Closing the context finalizes the video file and flushes it to disk.
        await Context.CloseAsync();

        await Browser.CloseAsync();
        Playwright.Dispose();
    }
}