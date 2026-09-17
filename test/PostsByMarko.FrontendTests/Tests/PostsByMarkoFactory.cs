using Microsoft.Playwright;
using PostsByMarko.FrontendTests.Drivers;
using PostsByMarko.Test.Shared.Helper;
using System.Diagnostics;
using System.Net;
using Xunit;

namespace PostsByMarko.FrontendTests.Tests;

public class PostsByMarkoFactory : IAsyncLifetime
{
    private readonly int timeoutInMs = (int)TimeSpan.FromSeconds(90).TotalMilliseconds;
    private static string? solutionPath;
    private bool startedCompose;

    public BrowserDriver? driver;
    public IBrowser browser = null!;

    public async Task InitializeAsync()
    {
        bool.TryParse(Environment.GetEnvironmentVariable("IsLocalDevelopment"), out var isLocalDevelopment);
        if (!isLocalDevelopment)
        {
            solutionPath = FileHelper.FindFileDirectory(Directory.GetCurrentDirectory(), "PostsByMarko.sln")
                ?? throw new InvalidOperationException("Could not locate the solution directory.");
            await RunComposeAsync("up", "--build", "--force-recreate", "-d");
            startedCompose = true;
            await WaitForHttpAsync("http://localhost:17171/index.html", response =>
                response.StatusCode == HttpStatusCode.OK && response.Body.Contains("swagger"));
            await WaitForHttpAsync("http://localhost:13000", response =>
                response.StatusCode == HttpStatusCode.OK && response.Body.Contains("id=\"app\""));
        }

        driver = new BrowserDriver();
        browser = await driver.GetFirefoxBrowserAsync();
    }

    public async Task DisposeAsync()
    {
        if (driver is not null) await driver.DestroyPlaywrightAsync();
        if (startedCompose) await RunComposeAsync("down", "--volumes", "--remove-orphans");
    }

    private static async Task RunComposeAsync(params string[] arguments)
    {
        var startInfo = new ProcessStartInfo("docker")
        {
            WorkingDirectory = solutionPath!,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };
        startInfo.ArgumentList.Add("compose");
        startInfo.ArgumentList.Add("-f");
        startInfo.ArgumentList.Add("docker-compose.test.yml");
        foreach (var argument in arguments) startInfo.ArgumentList.Add(argument);

        using var process = Process.Start(startInfo)
            ?? throw new InvalidOperationException("Could not start Docker Compose.");
        var standardOutput = process.StandardOutput.ReadToEndAsync();
        var standardError = process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();
        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException(
                $"Docker Compose failed with exit code {process.ExitCode}: {await standardError}\n{await standardOutput}");
        }
    }

    private async Task WaitForHttpAsync(string url, Func<(HttpStatusCode StatusCode, string Body), bool> ready)
    {
        using var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
        var deadline = DateTime.UtcNow.AddMilliseconds(timeoutInMs);
        Exception? lastException = null;
        while (DateTime.UtcNow < deadline)
        {
            try
            {
                using var response = await httpClient.GetAsync(url);
                var body = await response.Content.ReadAsStringAsync();
                if (ready((response.StatusCode, body))) return;
            }
            catch (Exception exception)
            {
                lastException = exception;
            }

            await Task.Delay(1000);
        }

        throw new TimeoutException($"Timed out waiting for {url}.", lastException);
    }
}
