Desktop Toast
=============

A library for toast notifications from desktop app. This library will, if necessary, install a shortcut in Windows startup and show a toast asynchronously.

##Requirements

 * .NET Framework 4.5
 * Windows 8.0 or newer

##Contents

 - DesktopToast: The library.

 - DesktopToast.Wpf: Sample WPF app to use this library.

 - DesktopToast.WinForms: Sample WinForms app to use this library.

 - DesktopToast.Proxy: Sample console app which acts as proxy to this library. This app will accept a request in JSON format from standard input, transfer the request to this library and return the result to standard output.

##Usage

Instantiate ToastRequest class, set its properties and then call ToastManager.ShowAsync method.

ToastRequest class is a container of information necessary for installing a shortcut and showing a toast. It has the following properties:

| Property                   | Description                                                                     | Note                  |
|----------------------------|---------------------------------------------------------------------------------|-----------------------|
| ToastHeadline              | Toast headline                                                                  | Optional              |
| ToastHeadlineWrapsTwoLines | Whether toast headline wraps across two lines                                   | Optional              |
| ToastBody                  | Toast body                                                                      | Required for toast    |
| ToastBodyExtra             | Toast body extra section                                                        | Optional              |
| ToastImageFilePath         | Toast image file path                                                           | Optional              |
| ToastAudio                 | Toast audio type                                                                | Optional              |
| ShortcutFileName           | Shortcut file name to be installed in Windows startup                           | Required for shortcut |
| ShortcutTargetFilePath     | Target file path of shortcut                                                    | Required for shortcut |
| ShortcutArguments          | Arguments of shortcut                                                           | Optional              |
| ShortcutComment            | Comment of shortcut                                                             | Optional              |
| ShortcutWorkingFolder      | Working folder of shortcut                                                      | Optional              |
| ShortcutWindowState        | Window state of shortcut                                                        | Optional              |
| ShortcutIconFilePath       | Icon file path of shortcut                                                      | Optional              |
| AppId                      | AppUserModelID of application                                                   | Required              |
| WaitingTime                | Waiting time length before showing a toast after the shortcut file is installed | Optional              |

##Other

 - License: MIT License