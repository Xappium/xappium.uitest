using Xunit;

namespace Xappium.Cli.Tests.Tools
{
    public class Tool
    {
        public Tool()
        {
            TestEnvironmentHost.Init();
        }
    }

    [CollectionDefinition(nameof(Tool), DisableParallelization = true)]
    public class ToolCollection : ICollectionFixture<Tool>
    {
    }
}
