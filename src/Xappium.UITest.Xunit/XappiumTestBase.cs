using System;
using Xunit;

namespace Xappium.UITest
{
    [Collection(nameof(XappiumTest))]
    public class XappiumTestBase : IDisposable
    {
        protected ITestEngine Engine { get; }

        public XappiumTestBase()
        {
            AppManager.StartApp();
            Engine = AppManager.Engine;
        }

        public virtual void Dispose()
        {
            Engine.StopApp();
        }
    }
}
