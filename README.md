# Xappium.UITest

The Xappium.UITest framework is a UITest helper framework built on top of Appium. This project is a combination of the work done independently by @dansiegel & @redth taking the best pieces of both.

This is meant to make it easier for Developers to write and integrate UI Tests with their mobile apps.

## Configuration

The UI Test AppManager is configured through a combination of local, embedded, and environment. This gives you a wide variety of configuration options and ways to customize your build. For example you may provide a `uitest.json` that contains common attributes for all builds... this will be automatically copied to the UI Test output directory. You may also conditionally embed a `uitest.ios.json` or `uitest.android.json` with the logical name `uitest.json`. Finally you may use various Environment variables to set the configuration dynamically in a CI build. The order of precedence is as follows:

- Local uitest.json on disk
- Embedded uitest.json in the UITest project assembly
- Environment Variables

Any of these properties can be set by you, however the Xappium.Client will automatically help you build out a completed config.

```json
{
  "platform": "Android",
  "appPath": "/Users/dansiegel/repos/Xappium.UITest/UITest/bin/com.avantipoint.testapp-Signed.apk",
  "appId": "com.avantipoint.testapp",
  "deviceName": "sdk_gphone_x86",
  "udid": "emulator-5554",
  "osVersion": "29",
  "settings": {
    "name": "Dan"
  },
  "capabilities": {
    "appWaitPackage": "com.avantipoint.testapp"
  }
}
```

### Settings vs Capabilities

At first it may be confusing that the configuration has both settings and capabilities. The Capabilities are explicit to the Appium Driver configuration. It is worth noting that if you need an explicit override to the way that Xappium is configuring a particular capability you have the ability to override that here.

Settings on the other hand have nothing specifically to do with Xappium.UITest or Appium, but rather are exposed to try to make it easier for you to provide values that you can use for your UI Tests such as user credentials.