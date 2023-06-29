using Xunit;

namespace PostsByMarko.FrontendTests
{

    [CollectionDefinition("Frontend Collection")]
    public class BaseCollection : ICollectionFixture<BaseFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
