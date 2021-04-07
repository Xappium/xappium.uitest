using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Xappium.UITest.MSTest
{
    [DoNotParallelize]
    public abstract class XappiumTestBase
    {
        protected ITestEngine Engine { get; private set; }

        [TestInitialize]
        public virtual void StartApp()
        {
            AppManager.StartApp();
            Engine = AppManager.Engine;
        }

        [TestCleanup]
        public virtual void CloseApp()
        {
            Engine.StopApp();
        }
    }
}
