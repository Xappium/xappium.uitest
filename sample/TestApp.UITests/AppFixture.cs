using System;
using Xappium.UITest;

namespace TestApp.UITests
{
    public sealed class AppFixture : IDisposable
    {
        public AppFixture()
        {
            AppManager.StartApp();
            Engine = AppManager.Engine;
        }

        public ITestEngine Engine { get; }

        public void Dispose()
        {
            Engine.StopApp();
        }
    }
}
