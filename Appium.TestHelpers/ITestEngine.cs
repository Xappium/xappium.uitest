using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Appium.TestHelpers
{
    public interface ITestEngine : IDisposable
    {
        Platform Platform { get; }

        IReadOnlyDictionary<string, string> Settings { get; }

        void StopApp();

        void Tap(string automationId);

        void TapAtIndex(string automationId, int index);

        void TapWithText(string automationId, string text);

        void TapWithText(string text);

        void TapWithClass(string automationId, string @class);

        void TapClass(string @class);

        void Screenshot(string title, [CallerMemberName] string methodName = null);

        void DismissKeyboard();

        void EnterText(string automationId, string text);

        void WaitForElementWithText(string text);

        void WaitForNoElementWithText(string text);

        void WaitForElement(string automationId);

        void WaitForNoElement(string automationId);

        void AssertTextInElementByAutomationId(string automationId, string text);

        void ScrollTo(string automationId);

        void TouchAndHoldWithText(string text);

        void SwipeToDelete(string automationId, int index);

        void WaitForAnyElement(params string[] automationIds);

        bool ElementExists(string automationId);

        void BackButton();
    }
}
