using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Xappium.UITest.Configuration;
using Xappium.UITest.Extensions;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.MultiTouch;
using OpenQA.Selenium.Support.UI;
using System.Reflection;
using OpenQA.Selenium.Interactions;
using Xappium.UITest.Providers;

namespace Xappium.UITest.Platforms
{
    internal abstract class XappiumTestEngineBase<T, E> : ITestEngine
        where T : AppiumDriver<E>
        where E : IWebElement
    {
        public const int SHORT_TIMEOUT = 5;
        public const int LONG_TIMEOUT = 30;

        private UITestConfiguration _config { get; }

        protected XappiumTestEngineBase(UITestConfiguration config)
        {
            _config = config;
            SetupDriver();
        }

        private void SetupDriver()
        {
            var options = new AppiumOptions();
            ConfigureCapabilities(options);

            Driver = CreateDriver(options, _config);

            // Setup timeouts
            Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(SHORT_TIMEOUT);
        }

        protected abstract T CreateDriver(AppiumOptions options, UITestConfiguration config);

        protected abstract IUIElement CreateUIElement(E nativeElement);

        protected WebDriverWait Wait(TimeSpan? timeout = null)
        {
            return new WebDriverWait(Driver, timeout ?? TimeSpan.FromSeconds(LONG_TIMEOUT));
        }

        public IReadOnlyDictionary<string, string> Settings => _config.Settings;

        public void Dispose()
            => StopApp();

        protected T Driver { get; private set; }

        public Platform Platform => _config.Platform;

        protected void AddAdditionalCapability(AppiumOptions options, string key, object value)
        {
            if (!_config.Capabilities.ContainsKey(key))
                options.AddAdditionalCapability(key, value);
        }

        private void ConfigureCapabilities(AppiumOptions options)
        {
            foreach ((string key, string value) in _config.Capabilities)
            {
                options.AddAdditionalCapability(key, value);
            }
        }

        public virtual void StopApp()
        {
            if (Driver != null)
            {
                Driver.Quit();
                Driver = null;
                AppManager.AppStopped();
            }
        }

        public virtual void Tap(string automationId, TimeSpan? timeout) =>
            Driver.FindElement(ByAutomationId(automationId))
                .Click();

        public virtual void TapAtIndex(string automationId, int index) =>
            Driver.FindElements(ByAutomationId(automationId))
                .ElementAt(index)
                .Click();

        public virtual void TapWithText(string automationId, string text)
        {
            foreach (var elem in Driver.FindElements(ByAutomationId(automationId)))
            {
                if (elem.Text.Equals(text, StringComparison.OrdinalIgnoreCase))
                {
                    elem.Click();
                    return;
                }
            }
        }

        public virtual void TapWithText(string text) =>
            Driver.FindElementByXPath("//*[@text='" + text + "']").Click();

        public virtual void TapWithClass(string automationId, string @class)
        {
            foreach (var elem in Driver.FindElements(ByAutomationId(automationId)))
            {
                if (elem.GetAttribute("className").Equals(@class, StringComparison.OrdinalIgnoreCase))
                {
                    elem.Click();
                    return;
                }
            }
        }

        public virtual void TapClass(string @class) =>
            Driver.FindElements(By.ClassName(@class))
                .FirstOrDefault()
                ?.Click();

        public virtual void Screenshot(string title, [CallerMemberName] string methodName = null)
        {
            var st = new StackTrace();
            var i = 1;
            MethodBase method = null;
            do
            {
                method = st.GetFrame(i++).GetMethod();
                if (method.ReflectedType.Assembly == GetType().Assembly)
                    method = null;
            } while (method is null);

            var className = method.ReflectedType.Name;
            var namespaceName = method.ReflectedType.Namespace;

            var baseDir = _config.ScreenshotsPath;

            var newFile = string.IsNullOrEmpty(title) ? $"{methodName}.png" : $"{methodName}-{title}.png";
            var newDir = Path.Combine(baseDir, Driver.SessionId.ToString(), namespaceName, className);
            var fullPath = Path.Combine(newDir, newFile);
            Directory.CreateDirectory(newDir);

            var s = Driver.GetScreenshot();
            s.SaveAsFile(fullPath, ScreenshotImageFormat.Png);
            TestFrameworkProvider.AttachFile(fullPath, Path.GetFileNameWithoutExtension(newFile));
        }

        public virtual void DismissKeyboard() =>
            Driver.HideKeyboard();

        public virtual IUIElement EnterText(string automationId, string text, TimeSpan? timeout)
        {
            var element = Driver.FindElement(ByAutomationId(automationId));
            element.SendKeys(text);
            return CreateUIElement(element);
        }

        public virtual IUIElement WaitForElementWithText(string text, TimeSpan? timeout)
        {
            var query = MobileBy.XPath("//*[@text='" + text + "']");
            Wait(timeout).Until(w => w.FindElement(query));
            return CreateUIElement(Driver.FindElement(query));
        }

        public virtual void WaitForNoElementWithText(string text, TimeSpan? timeout) =>
            Wait(timeout).Until(w => w.FindElements(MobileBy.XPath("//*[@text='" + text + "']")).Count <= 0);

        public virtual IUIElement WaitForElement(string automationId, TimeSpan? timeout)
        {
            var query = ByAutomationId(automationId);
            Wait(timeout).Until(w => w.FindElement(query));
            return CreateUIElement(Driver.FindElement(query));
        }

        public virtual void WaitForNoElement(string automationId, TimeSpan? timeout) =>
            Wait(timeout).Until(w => w.FindElements(ByAutomationId(automationId)).Count <= 0);

        public virtual void AssertTextInElementByAutomationId(string automationId, string text, TimeSpan? timeout)
        {
            var found = false;
            foreach (var elem in Driver.FindElements(ByAutomationId(automationId)))
            {
                if (elem.Text.IndexOf(text, StringComparison.InvariantCultureIgnoreCase) >= 0)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
                Providers.TestFrameworkProvider.Throw(automationId + " did not contain the text '" + text + "'");
        }

        public virtual void ScrollTo(string automationId)
        {
            var action = new TouchActions(Driver);
            action.Scroll(0, 100)
                .Perform();
        }

        public virtual IEnumerable<IUIElement> WaitForAnyElement(params string[] automationIds)
        {
            Wait().Until(w =>
                automationIds.Any(id => w.FindElements(ByAutomationId(id)).Any())
            );
            return automationIds.Select(x =>
                CreateUIElement(Driver.FindElement(ByAutomationId(x)))
            );
        }

        public virtual bool ElementExists(string automationId, TimeSpan? timeout) =>
            Driver.FindElements(ByAutomationId(automationId)).Any();

        public virtual void BackButton() =>
            Driver.Navigate().Back();

        public virtual void TouchAndHoldWithText(string text)
        {
            var e = Driver.FindElement(MobileBy.XPath("//*[@text='" + text + "']"));
            var touchAction = new TouchAction(Driver).LongPress(e);
            Driver.PerformTouchAction(touchAction);
        }

        public virtual void SwipeToDelete(string automationId, int index)
        {
            var item = Driver.FindElement(ByAutomationId(automationId));
            var width = Driver.Manage().Window.Size.Width;

            var act = new TouchAction(Driver);
            var press = act.Press(item, width, item.Location.Y);
            var move = act.MoveTo(item, 0, item.Location.Y);
            var lift = act.Release();

            var action = new MultiAction(Driver);
            action.Add(press);
            action.Add(move);
            action.Add(lift);
            action.Perform();
        }

        string GetTagKeyViewMatcherJson(int resourceId, string automationId) =>
            "{ \"name\" : \"withTagKey\", \"args\" : [ " + resourceId + ", { \"name\" : \"is\", \"args\" : \"" + automationId + "\" } ] }";

        public virtual By ByAutomationId(string automationId) =>
            MobileBy.AccessibilityId(automationId);
    }
}
