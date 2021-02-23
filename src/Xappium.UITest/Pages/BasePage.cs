namespace Xappium.UITest.Pages
{
    public abstract class BasePage
    {
        protected ITestEngine Engine => AppManager.Engine;
        protected bool OnAndroid => AppManager.Platform == Platform.Android;
        protected bool OniOS => AppManager.Platform == Platform.iOS;

        protected abstract string Trait { get; }

        protected string PageName { get; }

        protected BasePage()
        {
            PageName = GetType().Name;
            AssertOnPage();
        }

        /// <summary>
        /// Verifies that the trait is still present. Defaults to no wait.
        /// </summary>
        protected void AssertOnPage()
        {
            var message = "Unable to verify on page: " + PageName;

            Engine.WaitForElement(Trait);
            Engine.Screenshot("On " + PageName);
            //Assert.DoesNotThrow(() => Engine.WaitForElement(Trait), message);
        }

        /// <summary>
        /// Verifies that the trait is no longer present. Defaults to a 5 second wait.
        /// </summary>
        protected void WaitForPageToLeave()
        {
            //timeout = timeout ?? TimeSpan.FromSeconds(5);
            var message = "Unable to verify *not* on page: " + PageName;

            Engine.WaitForNoElement(Trait);
            //Assert.DoesNotThrow(() => Engine.WaitForElement(Trait), message);
        }
    }
}
