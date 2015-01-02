using System;
using System.IO;

namespace DesktopToast.Helper
{
    internal class Shortcut
    {
        /// <summary>
        /// Check if a specified shortcut file exists.
        /// </summary>
        /// <param name="shortcutPath">Shortcut file path</param>
        /// <param name="targetPath">Target file path of shortcut</param>
        /// <param name="arguments">Arguments of shortcut</param>
        /// <param name="appId">AppUserModelID of shortcut</param>
        /// <returns>True if exists</returns>
        public bool CheckShortcut(string shortcutPath, string targetPath, string arguments, string appId)
        {
            if (!File.Exists(shortcutPath))
                return false;

            using (var shellLink = new ShellLink(shortcutPath))
            {
                // File path casing may be different from that when installed the shortcut file.
                return (shellLink.TargetPath.EqualsOrIsNullOrEmpty(targetPath, StringComparison.OrdinalIgnoreCase) &&
                    shellLink.Arguments.EqualsOrIsNullOrEmpty(arguments, StringComparison.Ordinal) &&
                    shellLink.AppUserModelID.EqualsOrIsNullOrEmpty(appId, StringComparison.Ordinal));
            }
        }

        /// <summary>
        /// Install a specified shortcut file.
        /// </summary>
        /// <param name="shortcutPath">Shortcut file path</param>
        /// <param name="targetPath">Target file path of shortcut</param>
        /// <param name="arguments">Arguments of shortcut</param>
        /// <param name="appId">AppUserModelID of shortcut</param>
        /// <param name="iconPath">Icon file path of shortcut</param>
        public void InstallShortcut(string shortcutPath, string targetPath, string arguments, string appId, string iconPath)
        {
            if (String.IsNullOrWhiteSpace(shortcutPath))
                throw new ArgumentNullException("shortcutPath");

            using (var shellLink = new ShellLink
            {
                TargetPath = targetPath,
                Arguments = arguments,
                AppUserModelID = appId,
                IconPath = iconPath,
                IconIndex = 0, // 1st icon in the file
                WindowStyle = ShellLink.SW.SW_SHOWNORMAL,
            })
            {
                shellLink.Save(shortcutPath);
            }
        }

        /// <summary>
        /// Delete a specified shortcut file.
        /// </summary>
        /// <param name="shortcutPath">Shortcut file path</param>
        /// <param name="targetPath">Target file path of shortcut</param>
        /// <param name="arguments">Arguments of shortcut</param>
        /// <param name="appId">AppUserModelID of shortcut</param>
        /// <remarks>If contents of shortcut does not match the shortcut file will not be deleted.</remarks>
        public void DeleteShortcut(string shortcutPath, string targetPath, string arguments, string appId)
        {
            if (!CheckShortcut(shortcutPath, targetPath, arguments, appId))
                return;

            File.Delete(shortcutPath);
        }
    }
}