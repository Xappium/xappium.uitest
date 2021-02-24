using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Xappium.UITest
{
    public interface ITestEngine : IDisposable
    {
        Platform Platform { get; }

        /// <summary>
        /// Returns Settings provided by your configuration to help with testing
        /// </summary>
        /// <remarks>
        /// You may use this to provide values such as user credentials
        /// </remarks>
        IReadOnlyDictionary<string, string> Settings { get; }

        void StopApp();

        void Tap(string automationId, TimeSpan? timeout);

        void TapAtIndex(string automationId, int index);

        void TapWithText(string automationId, string text);

        void TapWithText(string text);

        void TapWithClass(string automationId, string @class);

        void TapClass(string @class);

        void Screenshot(string title, [CallerMemberName] string methodName = null);

        void DismissKeyboard();

        IUIElement EnterText(string automationId, string text, TimeSpan? timeout);

        IUIElement WaitForElementWithText(string text, TimeSpan? timeout);

        void WaitForNoElementWithText(string text, TimeSpan? timeout);

        IUIElement WaitForElement(string automationId, TimeSpan? timeout);

        void WaitForNoElement(string automationId, TimeSpan? timeout);

        void AssertTextInElementByAutomationId(string automationId, string text, TimeSpan? timeout);

        void ScrollTo(string automationId);

        void TouchAndHoldWithText(string text);

        void SwipeToDelete(string automationId, int index);

        IEnumerable<IUIElement> WaitForAnyElement(params string[] automationIds);

        bool ElementExists(string automationId, TimeSpan? timeout);

        void BackButton();
    }
}
