# Desktop Toast

A library for toast notifications from desktop app. This library will, if necessary, install a shortcut in Windows startup and show a toast asynchronously.

## Requirements

 * .NET Framework 4.5.2
 * Windows 8.0 or newer

## Contents

 - DesktopToast - The library.

 - DesktopToast.Wpf - Sample WPF app to use this library.

 - DesktopToast.WinForms - Sample WinForms app to use this library.

 - DesktopToast.Proxy - Sample console app which acts as proxy to this library. This app will accept a request in JSON format from standard input, transfer the request to this library and return the result to standard output.

## Usage

Instantiate ToastRequest class, set its properties and then call ToastManager.ShowAsync method.

```csharp
public async Task<bool> ShowToastAsync()
{
    var request = new ToastRequest
    {
        ToastTitle = "DesktopToast WPF Sample",
        ToastBody = "This is a toast test.",
        ToastLogoFilePath = string.Format("file:///{0}", Path.GetFullPath("toast128.png")),
        ShortcutFileName = "DesktopToast.Wpf.lnk",
        ShortcutTargetFilePath = Assembly.GetExecutingAssembly().Location,
        AppId = "DesktopToast.Wpf",
    };

    var result = await ToastManager.ShowAsync(request);

    return (result == ToastResult.Activated);
}
```

ToastRequest class is a container of information necessary for installing a shortcut and showing a toast. It has the following properties:

| Property               | Description                                                                             | Note                  |
|------------------------|-----------------------------------------------------------------------------------------|-----------------------|
| ToastTitle             | Toast title                                                                             | Optional              |
| ToastBody              | Toast body                                                                              | Required for toast    |
| ToastBodyList          | Toast body list (If specified, toast body will be substituted by this list.)            | Optional              |
| ToastLogoFilePath      | Logo image file path of toast                                                           | Optional              |
| ToastAudio             | Audio type of toast                                                                     | Optional              |
| ToastXml               | XML representation of Toast (If specified, this XML will be used for a toast as it is.) | Optional              |
| ShortcutFileName       | Shortcut file name to be installed in Windows startup                                   | Required for shortcut |
| ShortcutTargetFilePath | Target file path of shortcut                                                            | Required for shortcut |
| ShortcutArguments      | Arguments of shortcut                                                                   | Optional              |
| ShortcutComment        | Comment of shortcut                                                                     | Optional              |
| ShortcutWorkingFolder  | Working folder of shortcut                                                              | Optional              |
| ShortcutWindowState    | Window state of shortcut                                                                | Optional              |
| ShortcutIconFilePath   | Icon file path of shortcut                                                              | Optional              |
| AppId                  | AppUserModelID of application                                                           | __Required__          |
| ActivatorId            | AppUserModelToastActivatorCLSID of application (for Action Center of Windows 10)        | Optional              |
| WaitingDuration        | Waiting duration before showing a toast after the shortcut file is installed            | Optional              |

## Action Center of Windows 10

To interact with Action Center of Windows 10, an application needs to register COM class type which implements [INotificationActivationCallback][1]. In addition, the registration of COM server in the registry is required for an application to be started by COM when it is not running.

See WPF sample for implementation. Note that the CLSID of COM class type (AppUserModelToastActivatorCLSID) must be unique for each application.

Also check the following sample.

 * [WindowsNotifications/desktop-toasts][2]

## Interactive toast of Windows 10

To show an interactive toast of Windows 10, prepare a XML representation of toast and set it to ToastXml property. Check the following article.

 * [Adaptive and interactive toast notifications for Windows 10][3]

You can compose it from scratch or utilize [NotificationsExtensions.Win10][4] library. See WPF sample.

## License

 - MIT License

[1]: https://msdn.microsoft.com/en-us/library/windows/desktop/mt643711.aspx
[2]: https://github.com/WindowsNotifications/desktop-toasts
[3]: https://blogs.msdn.microsoft.com/tiles_and_toasts/2015/07/02/adaptive-and-interactive-toast-notifications-for-windows-10/
[4]: https://www.nuget.org/packages/NotificationsExtensions.Win10/