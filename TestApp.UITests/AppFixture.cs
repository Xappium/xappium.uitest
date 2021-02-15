using System;
using Appium.TestHelpers;

namespace TestApp.UITests
{
    public sealed class AppFixture : IDisposable
    {
        public AppFixture()
        {
            AppManager.StartApp();
            Engine = AppManager.Engine;
            // App.LaunchApp();
        }

        public ITestEngine Engine { get; }

        public void Dispose()
        {
            // App.Quit();
        }
    }
}
