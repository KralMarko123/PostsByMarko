using aspnetserver.Helper;
using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Services;
using Xunit;

namespace PostsTesting.Tests
{
    public class TestingFactory : IAsyncLifetime
    {
        private static readonly string solutionDirectory = VisualStudioHelper.TryGetSolutionDirectoryInfo().FullName;
        private static readonly string dockerComposeFilePath = Path.Combine(solutionDirectory, "docker-compose.yml");
        private readonly ICompositeService dockerBuilder = new Builder()
                    .UseContainer()
                    .UseCompose()
                    .FromFile(dockerComposeFilePath)
                    .RemoveOrphans()
                    .WaitForHttp("postsdb", "http://localhost:1433")
                    .WaitForHttp("aspnetserver", "http://localhost:7171")
                    .WaitForHttp("reactclient", "http://localhost:3000")
                    .Build();
        public async Task InitializeAsync()
        {
            dockerBuilder.Start();
        }

        public async Task DisposeAsync()
        {
            dockerBuilder.Stop();
            dockerBuilder.Dispose();
        }
    }
}
