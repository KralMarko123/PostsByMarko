﻿using Xunit;

namespace PostsByMarko.IntegrationTests
{

    [CollectionDefinition("Integration Collection")]
    public class PostsByMarkoCollection : ICollectionFixture<PostsByMarkoApiFactory>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}