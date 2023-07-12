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
        private BrowserDriver? driver;
        private IBrowser? browser;
        public IPage? page;

        private static ICompositeService? dockerServices;

        public async Task InitializeAsync()
        {
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

        private void InitializeDockerContainers()
        {
            var path = FileHelper.FindFileDirectory(Directory.GetCurrentDirectory(), "PostsByMarko.sln")!;
            var composeFiles = Directory.GetFiles(path, "*compose*.yml");
            var timeoutInMs = TimeSpan.FromSeconds(30).Milliseconds;
            var serviceBuilder = new Builder()
                .UseContainer()
                .UseCompose()
                .FromFile(composeFiles)
                .ForceRecreate()
                .RemoveOrphans()
                .WaitForPort("PostsByMarko.Host", "7171/tcp", timeoutInMs)
                .WaitForHttp("PostsByMarko.Host", "http://localhost:7171/index.html", timeoutInMs, continuation: (response, count) => CheckStatusCode(response, count))
                .WaitForPort("PostsByMarko.Client", "3000/tcp", timeoutInMs)
                .WaitForHttp("PostsByMarko.Client", "http://localhost:3000/login", timeoutInMs, continuation: (response, count) => CheckStatusCode(response, count));

            dockerServices = serviceBuilder.Build();
            dockerServices.Start();
        }

        private static int CheckStatusCode(RequestResponse response, int count)
        {
            if (count > 5) return 0;

            return response.Code == HttpStatusCode.OK ? 0 : 10000;
        }
    }
}
