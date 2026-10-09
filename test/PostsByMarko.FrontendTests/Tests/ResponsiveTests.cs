using Microsoft.Playwright;
using PostsByMarko.Test.Shared.Constants;
using Xunit;
using static Microsoft.Playwright.Assertions;

namespace PostsByMarko.FrontendTests.Tests;

[Collection("Frontend Collection")]
public class ResponsiveTests
{
    private readonly PostsByMarkoFactory factory;

    public ResponsiveTests(PostsByMarkoFactory factory)
    {
        this.factory = factory;
    }

    [Theory]
    [InlineData(320)]
    [InlineData(390)]
    [InlineData(900)]
    [InlineData(1440)]
    public async Task should_support_keyboard_navigation_and_dialogs_at_each_viewport(int width)
    {
        // Arrange
        await using var context = await factory.browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = width, Height = 900 }
        });
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{TestingConstants.DEV_CLIENT_ENDPOINT}/login");
        await CaptureAsync(page, $"login-{width}");
        await page.GetByLabel("Email", new PageGetByLabelOptions { Exact = true }).FillAsync(TestingConstants.TEST_ADMIN_EMAIL);
        var password = page.GetByLabel("Password", new PageGetByLabelOptions { Exact = true });
        await password.FillAsync(TestingConstants.TEST_PASSWORD);

        // Act
        await password.PressAsync("Enter");
        await Expect(page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Today's Posts" })).ToBeVisibleAsync();
        await Expect(page.Locator(".post").First).ToBeVisibleAsync();

        // Assert
        await AssertNoOverflowAsync(page);
        Assert.Equal(width < 768 ? 1 : width < 1280 ? 2 : 3, await page.Locator(".posts-list").EvaluateAsync<int>("element => getComputedStyle(element).gridTemplateColumns.split(' ').length"));
        await CaptureAsync(page, $"feed-{width}");

        // Act
        var menu = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Menu", Exact = true });
        await menu.FocusAsync();
        await menu.PressAsync("Enter");
        var create = page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Create Post", Exact = true });
        await create.FocusAsync();
        await create.PressAsync("Enter");
        var dialog = page.GetByRole(AriaRole.Dialog, new PageGetByRoleOptions { Name = "Create post", Exact = true });

        // Assert
        await Expect(dialog).ToBeVisibleAsync();
        await Expect(dialog.GetByLabel("Title", new LocatorGetByLabelOptions { Exact = true })).ToBeFocusedAsync();
        await AssertNoOverflowAsync(page);
        for (var index = 0; index < 6; index++)
        {
            await page.Keyboard.PressAsync("Tab");
            Assert.True(await page.EvaluateAsync<bool>("() => document.activeElement.closest('dialog') !== null"));
        }
        await CaptureAsync(page, $"dialog-{width}");

        // Act
        await page.Keyboard.PressAsync("Escape");

        // Assert
        await Expect(dialog).ToBeHiddenAsync();
        await Expect(menu).ToBeFocusedAsync();

        // Act
        await menu.PressAsync("Enter");
        await page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = "Chat", Exact = true }).ClickAsync();

        // Assert
        await Expect(page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Chat", Exact = true })).ToBeVisibleAsync();
        await Expect(page.Locator(".user-card").First).ToBeVisibleAsync();
        await AssertNoOverflowAsync(page);
        await CaptureAsync(page, $"chat-{width}");

        // Act
        await page.Locator(".user-card").First.ClickAsync();

        // Assert
        await Expect(page.GetByRole(AriaRole.Textbox, new PageGetByRoleOptions { Name = "Message", Exact = true })).ToBeVisibleAsync();
        await AssertNoOverflowAsync(page);
        await CaptureAsync(page, $"conversation-{width}");

        // Act
        await menu.ClickAsync();
        await page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = "Dashboard", Exact = true }).ClickAsync();

        // Assert
        await Expect(page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Admin Dashboard" })).ToBeVisibleAsync();
        await Expect(page.GetByRole(AriaRole.Table, new PageGetByRoleOptions { Name = "User management" })).ToBeVisibleAsync();
        await AssertNoOverflowAsync(page);
        await CaptureAsync(page, $"admin-{width}");

        // Act
        var actions = page.GetByRole(AriaRole.Columnheader, new PageGetByRoleOptions { Name = "Actions", Exact = true });
        await actions.ScrollIntoViewIfNeededAsync();

        // Assert
        await Expect(actions).ToBeInViewportAsync();
        await AssertNoOverflowAsync(page);
    }

    [Fact]
    public async Task should_discard_cancelled_edits_when_reopening_the_same_post()
    {
        // Arrange
        await using var context = await factory.browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await page.GotoAsync($"{TestingConstants.DEV_CLIENT_ENDPOINT}/login");
        await page.GetByLabel("Email", new PageGetByLabelOptions { Exact = true }).FillAsync(TestingConstants.TEST_ADMIN_EMAIL);
        await page.GetByLabel("Password", new PageGetByLabelOptions { Exact = true }).FillAsync(TestingConstants.TEST_PASSWORD);
        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Sign In", Exact = true }).ClickAsync();
        var post = page.Locator(".post").First;
        await Expect(post).ToBeVisibleAsync();
        var originalTitle = await post.Locator(".post-title").TextContentAsync();
        var edit = post.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = $"Edit post: {originalTitle}", Exact = true });

        // Act
        await edit.ClickAsync();
        var dialog = page.GetByRole(AriaRole.Dialog, new PageGetByRoleOptions { Name = "Update post", Exact = true });
        await dialog.GetByLabel("Title", new LocatorGetByLabelOptions { Exact = true }).FillAsync("Discard this draft");
        await dialog.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Cancel", Exact = true }).ClickAsync();
        await edit.ClickAsync();

        // Assert
        await Expect(dialog.GetByLabel("Title", new LocatorGetByLabelOptions { Exact = true })).ToHaveValueAsync(originalTitle!);
        await Expect(post.Locator(".post-title")).ToHaveTextAsync(originalTitle!);
    }

    private static async Task AssertNoOverflowAsync(IPage page)
    {
        Assert.True(await page.EvaluateAsync<bool>("() => document.documentElement.scrollWidth <= window.innerWidth"), "The page must fit the viewport without horizontal scrolling.");
    }

    private static async Task CaptureAsync(IPage page, string name)
    {
        var directory = Environment.GetEnvironmentVariable("FRONTEND_SCREENSHOT_DIRECTORY");
        if (string.IsNullOrWhiteSpace(directory))
            return;
        Directory.CreateDirectory(directory);
        await page.EvaluateAsync("async () => { await document.fonts.ready; }");
        await page.ScreenshotAsync(new PageScreenshotOptions { Path = Path.Combine(directory, $"{name}.png"), FullPage = true, Animations = ScreenshotAnimations.Disabled });
    }
}
