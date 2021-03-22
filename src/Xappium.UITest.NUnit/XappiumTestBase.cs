using NUnit.Framework;

namespace Xappium.UITest
{
    public class XappiumTestBase
    {
        protected ITestEngine Engine { get; private set; }

        [SetUp]
        public virtual void StartApp()
        {
            AppManager.StartApp();
            Engine = AppManager.Engine;
        }

        [TearDown]
        public virtual void CloseApp()
        {
            Engine.StopApp();
        }
    }
}
