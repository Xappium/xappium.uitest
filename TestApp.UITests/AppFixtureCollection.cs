using Xunit;

namespace TestApp.UITests
{
    [CollectionDefinition(nameof(AppFixture))]
    public sealed class AppFixtureCollection : ICollectionFixture<AppFixture>
    {
    }
}
