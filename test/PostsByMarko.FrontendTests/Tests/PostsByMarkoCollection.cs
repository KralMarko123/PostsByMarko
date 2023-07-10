using Xunit;

namespace PostsByMarko.FrontendTests.Tests
{

    [CollectionDefinition("Frontend Collection")]
    public class PostsByMarkoCollection : ICollectionFixture<PostsByMarkoHostFactory>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
