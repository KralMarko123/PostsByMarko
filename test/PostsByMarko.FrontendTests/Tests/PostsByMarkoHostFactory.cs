using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Images;
using Microsoft.Playwright;
using PostsByMarko.FrontendTests.Drivers;
using PostsByMarko.Test.Shared.Helper;
using Xunit;

namespace PostsByMarko.FrontendTests.Tests
{
    public class PostsByMarkoHostFactory : IAsyncLifetime
    {
        private BrowserDriver? driver;
        private IBrowser? browser;
        public IPage page;

        private string? hostDockerfilePath;
        private IFutureDockerImage? hostImage;
        private IContainer? hostContainer;


        public async Task InitializeAsync()
        {
            ReadDockerfile();
            SetupHostImageAndContainer();
            await hostImage!.CreateAsync().ConfigureAwait(false);
            await hostContainer!.StartAsync().ConfigureAwait(false);

            driver = new BrowserDriver();
            browser = await driver.GetChromeBrowserAsync();
            page = await browser.NewPageAsync();
        }

        public async Task DisposeAsync()
        {
            await page.CloseAsync();
            await browser!.CloseAsync();
            await browser.DisposeAsync();
            await driver!.DestroyPlaywrightAsync();
        }

        private void ReadDockerfile()
        {
            var slnPath = FileHelper.FindFileDirectory(Directory.GetCurrentDirectory(), "PostsByMarko.sln");
            var hostFiles = Directory.GetFiles(Path.Combine(slnPath!, $"src\\PostsByMarko.Host"));

            hostDockerfilePath = hostFiles.First(f => f.EndsWith("Dockerfile"));
        }

        private void SetupHostImageAndContainer()
        {
            try
            {
                hostImage = new ImageFromDockerfileBuilder()
                .WithName("postsbymarko.host")
                .WithDockerfile(hostDockerfilePath)
                .WithDeleteIfExists(false)
                .Build();

                hostContainer = new ContainerBuilder()
                .WithImage(hostImage.FullName)
                .WithPortBinding(7171, 7171)
                .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
                .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(7171))
                .Build();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
