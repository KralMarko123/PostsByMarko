using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Xunit;

namespace PostsTesting.Tests
{
    public class TestingFactory : IAsyncLifetime
    {
        private readonly TestcontainersContainer clientContainer = new TestcontainersBuilder<MsSqlTestcontainer>()
           .WithName("posts-client")
           .WithImage("reactclient:latest")
           .WithPortBinding(3000, true)
           .Build();

        private readonly TestcontainersContainer serverContainer = new TestcontainersBuilder<TestcontainersContainer>()
            .WithName("posts-server")
            .WithImage("aspnetserver:latest")
            .WithExposedPort(80)
            .WithPortBinding(7171)
            .Build();

        private readonly TestcontainersContainer clientContainer = new TestcontainersBuilder<TestcontainersContainer>()
           .WithName("posts-client")
           .WithImage("reactclient:latest")
           .WithPortBinding(3000, true)
           .Build();

        public async Task InitializeAsync()
        {
            throw new NotImplementedException();
        }

        public async Task DisposeAsync()
        {
            throw new NotImplementedException();
        }


    }
}
