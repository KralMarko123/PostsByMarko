using Ductus.FluentDocker.Services;
using Microsoft.Playwright;
using PostsByMarko.FrontendTests.Drivers;
using PostsByMarko.Test.Shared.Helper;
using Xunit;
using Ductus.FluentDocker.Builders;
using System.Net;
using Ductus.FluentDocker.Common;

namespace PostsByMarko.FrontendTests.Tests
{
    public class PostsByMarkoFactory : IAsyncLifetime
    {
        private static readonly int timeoutInMs = TimeSpan.FromSeconds(30).Milliseconds;

        private BrowserDriver? driver;
        private IBrowser? browser;
        public IPage? page;

        private static string[]? composeFiles;
        private static ICompositeService? dockerServices;

        public async Task InitializeAsync()
        {
            ReadComposeFiles();
            InitializeDockerContainers();

            driver = new BrowserDriver();
            browser = await driver.GetChromeBrowserAsync();
            page = await browser.NewPageAsync();
        }

        public async Task DisposeAsync()
        {
            await page!.CloseAsync();
            await browser!.CloseAsync();
            await browser.DisposeAsync();
            await driver!.DestroyPlaywrightAsync();

            dockerServices!.Stop();
            dockerServices.Dispose();
        }

        private static void ReadComposeFiles()
        {
            var solutionPath = FileHelper.FindFileDirectory(Directory.GetCurrentDirectory(), "PostsByMarko.sln")!;
            composeFiles = Directory.GetFiles(solutionPath, "*compose*.yml");
        }

        private static void InitializeDockerContainers()
        {
            try
            {
                dockerServices = new Builder()
                    .UseContainer()
                    .UseCompose()
                    .FromFile(composeFiles)
                    .ForceBuild()
                    .ForceRecreate()
                    .RemoveOrphans()
                    .WaitForHttp("PostsByMarko.Host", "http://localhost:7171/index.html", timeoutInMs, continuation: (response, count) => CheckStatusCode(response, count))
                    .WaitForHttp("PostsByMarko.Client", "http://localhost:3000/login", timeoutInMs, continuation: (response, count) => CheckStatusCode(response, count))
                    .Build();

                dockerServices.Start();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static int CheckStatusCode(RequestResponse response, int count)
        {
            if (count > 5) return 0;

            return response.Code == HttpStatusCode.OK ? 0 : 10000;
        }
    }
}
