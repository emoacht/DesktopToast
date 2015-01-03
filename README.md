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

 - ToastHeadline: Toast headline (optional)
 - ToastHeadlineWrapsTwoLines: Whether toast headline wraps across two lines (optional)
 - ToastBody: Toast body (required for toast)
 - ToastBodyExtra: Toast body extra section (optional)
 - ToastImageFilePath: Toast image file path (optional)
 - ToastAudio: Toast audio type (optional)

 - ShortcutFileName: Shortcut file name to be installed in Windows startup (required for shortcut)
 - ShortcutTargetFilePath: Target file path of shortcut (required for shortcut)
 - ShortcutArguments: Arguments of shortcut (optional)
 - ShortcutComment: Comment of shortcut (optional)
 - ShortcutWorkingFolder: Working folder of shortcut (optional)
 - ShortcutWindowState: Window state of shortcut (optional)
 - ShortcutIconFilePath: Icon file path of shortcut (optional)

 - AppId: AppUserModelID of application (required)
 - WaitingTime: Waiting time length before showing a toast after the shortcut file is installed (optional)

##Other

 - License: MIT License