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
        private int timeoutInMs = TimeSpan.FromSeconds(20).Milliseconds;

        public BrowserDriver? driver;

        private static string[]? composeFiles;
        private static ICompositeService? dockerServices;

        public async Task InitializeAsync()
        {
            ReadComposeFiles();
            InitializeDockerContainersThroughCompose();

            driver = new BrowserDriver();
        }

        public async Task DisposeAsync()
        {
            await driver!.DestroyPlaywrightAsync();

            dockerServices!.Stop();
            dockerServices.Dispose();
        }

        private void ReadComposeFiles()
        {
            var solutionPath = FileHelper.FindFileDirectory(Directory.GetCurrentDirectory(), "PostsByMarko.sln")!;
            composeFiles = Directory.GetFiles(solutionPath, "*compose*.yml");
        }

        private void InitializeDockerContainersThroughCompose()
        {
            try
            {
                dockerServices = new Builder()
                    .UseContainer()
                    .UseCompose()
                    .FromFile(composeFiles)
                    .ForceBuild()
                    .ForceRecreate()
                    .WaitForHttp("PostsByMarko.Host", "http://localhost:7171/index.html", timeoutInMs, (response, retryIn) => CheckSwaggerIsEnabled(response))
                    .WaitForHttp("PostsByMarko.Client", "http://localhost:3000", timeoutInMs, (response, retryIn) => CheckForIconOnUI(response))
                    .Build();

                dockerServices.Start();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private int CheckSwaggerIsEnabled(RequestResponse response)
        {
            return response.Code == HttpStatusCode.OK && response.Body.Contains("swagger") ? 0 : 1000;
        }

        private int CheckForIconOnUI(RequestResponse response)
        {
            return response.Code == HttpStatusCode.OK && response.Body.Contains("app") ? 0 : 1000;
        }
    }
}
