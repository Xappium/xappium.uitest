# Xappium.UITest

Writing Cross Platform (X-Plat) apps can be a challenge. Finding a way to more easily write and run integrated UI Tests can be even more challenging. Xappium.UITest aims to help solve some of these issues by building an additional layer of abstraction over Appium that makes it easier to write X-Plat UI Tests. This project is a combination of the work done independently by [@dansiegel](https://github.com/sponsors/dansiegel) & [@redth](https://github.com/redth) taking the best pieces of both.

This is meant to make it easier for Developers to write and integrate UI Tests with their mobile apps.

![Xappium UITest!](resources/social-preview.png "Xappium UITest")

## Current State

This repo is currently evolving and may experience breaking API changes. Community feedback and contributions are welcome and encouraged. We hope that this project will serve both as a model and backbone for UI Testing for .NET Developers with classic Xamarin / Xamarin.Forms applications as well as newer .NET 6 / .NET MAUI applications.

### Roadmap

| Description | Status |
|-------------|:------:|
| iOS Support | Done |
| Android Support | Done |
| WinUI Support | PR Welcome |
| macOS Support | PR Welcome |
| GTK Support | PR Welcome |
| Tizen Support | PR Welcome |
| CLI tool | [In process](https://github.com/Xappium/xappium.cli) |

**NOTE:** Android tools are tested and do work on Mac, however they do not currently work on Windows hosts. If you're an Android Guru and would like to help get this working on Windows hosts I'd love the PR!

## Configuration

The UI Test AppManager is configured through a combination of local, embedded, and environment. This gives you a wide variety of configuration options and ways to customize your build. For example you may provide a `uitest.json` that contains common attributes for all builds... this will be automatically copied to the UI Test output directory. You may also conditionally embed a `uitest.ios.json` or `uitest.android.json` with the logical name `uitest.json`. Finally you may use various Environment variables to set the configuration dynamically in a CI build. The order of precedence is as follows:

- Local uitest.json on disk (this can be overridden in the CLI)
- Embedded uitest.json in the UITest project assembly
- Environment Variables

Any of these properties can be set by you, however the [Xappium.Cli](https://github.com/Xappium/xappium.cli) will automatically help you build out a completed config.

```json
{
  "platform": "Android",
  "appPath": "/<Project Path>/UITest/bin/com.avantipoint.testapp-Signed.apk",
  "appId": "com.avantipoint.testapp",
  "deviceName": "sdk_gphone_x86",
  "udid": "emulator-5554",
  "osVersion": "29",
  "settings": {
    "name": "Dan"
  },
  "capabilities": {
    "adbPort": "5037"
  }
}
```

### Settings vs Capabilities

At first it may be confusing that the configuration has both settings and capabilities. The Capabilities are explicit to the Appium Driver configuration. It is worth noting that if you need an explicit override to the way that Xappium is configuring a particular capability you have the ability to override that here. Note that the adbPort shown above is provided as an example and should not be required for Android.

Settings on the other hand have nothing specifically to do with Xappium.UITest or Appium, but rather are exposed to try to make it easier for you to provide values that you can use for your UI Tests such as user credentials.

### Writing UI Tests with Xappium

Xappium provides helpers for MSTest, NUnit, & Xunit. These each have a `XappiumTestBase` class that will properly handle starting and stopping the app between each test. This ensures that Appium assigns a new Test Session between each of your tests. As a result the screenshots you take will be protected from accidentally being overwritten from one test to the next.

We'll set up our tests using the Page-Object-Model. You can get more information on how this works from [Sweeky's talk](https://www.youtube.com/watch?v=4VR861BWkiU) at the Xamarin Developer Summit. To do this we'll use a helper class from Xappium.UITest:

```cs
public class LoginPage : BasePage
{
    protected override string Trait => "LoginButton";
}
```

You'll notice that we provide a trait. This is generally the AutomationId of an element on the page that should be unique that page. Now let's add a test class to validate that our App Starts up and is on the LoginPage.

```cs
public class AppTests : XappiumTestBase
{
    [Fact]
    public void AppLaunches()
    {
        new LoginPage();
    }
}
```

#### Picking a Unit Test Framework

Xappium UITest actually has no requirements on which test framework you need to use as long as it's compatible with `dotnet test`. As a result you are able to write your UI Tests using your favorite testing frameworks.

## Using the Cli Tool

**NOTE:** The Cli has moved to a [new repo](https://github.com/Xappium/xappium.cli). Xappium.UITest works with or without the Cli. The Cli is meant to make running tests a breeze by taking care of all complexities of running tests and even build the UI Test and App Head if needed.
