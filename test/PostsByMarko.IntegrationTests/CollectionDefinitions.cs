using Xunit;

namespace PostsByMarko.IntegrationTests
{

    [CollectionDefinition("Sequential Collection", DisableParallelization = true)]
    public class SequentialCollection : ICollectionFixture<PostsByMarkoApiFactory>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
