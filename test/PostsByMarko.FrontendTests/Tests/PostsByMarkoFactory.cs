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
                .WaitForPort("host", "7171/tcp", timeoutInMs)
                .WaitForPort("client", "3000/tcp", timeoutInMs)
                .WaitForHttp("host", "http://localhost:7171/index.html", timeoutInMs)
                .WaitForHttp("client", "http://localhost:3000/login", timeoutInMs);


            dockerServices = serviceBuilder.Build();
            dockerServices.Start();
        }

        private static int CheckStatusCode(RequestResponse resp) => resp.Code == HttpStatusCode.OK ? 0 : 10000;
    }
}
