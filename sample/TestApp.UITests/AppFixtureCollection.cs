using Xunit;

namespace TestApp.UITests
{
    [CollectionDefinition(nameof(AppFixture), DisableParallelization = true)]
    public sealed class AppFixtureCollection : ICollectionFixture<AppFixture>
    {
    }
}
