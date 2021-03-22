using System.ComponentModel;
using Xunit;

namespace Xappium.UITest
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    [CollectionDefinition(nameof(XappiumTest), DisableParallelization = true)]
    public class XappiumTestCollection : ICollectionFixture<XappiumTest>
    {
    }
}
