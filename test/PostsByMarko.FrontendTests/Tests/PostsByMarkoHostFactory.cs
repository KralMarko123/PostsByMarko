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

        private const string hostDockerfilePathFromSln = "src/PostsByMarko.Host/Dockerfile";
        private IFutureDockerImage? hostImage;
        private IContainer? hostContainer;


        public async Task InitializeAsync()
        {
            await SetupHostImageAndContainer();

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

        private async Task SetupHostImageAndContainer()
        {
            try
            {
                hostImage = new ImageFromDockerfileBuilder()
                .WithName("postsbymarko.host:0.0.1")
                .WithDockerfileDirectory(CommonDirectoryPath.GetSolutionDirectory(), string.Empty)
                .WithDockerfile(hostDockerfilePathFromSln)
                .WithDeleteIfExists(false)
                .WithCleanUp(true)
                .WithBuildArgument("RESOURCE_REAPER_SESSION_ID", ResourceReaper.DefaultSessionId.ToString("D"))
                .Build();

                await hostImage.CreateAsync().ConfigureAwait(false);

                hostContainer = new ContainerBuilder()
                .WithName("postsbymarkohost-container")
                .WithImage(hostImage.FullName)
                .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
                .WithPortBinding(7171, 7171)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(7171))
                .WithCleanUp(true)
                .Build();

                await hostContainer.StartAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
