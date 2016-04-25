using System;
using System.IO;

namespace DesktopToast.Helper
{
	/// <summary>
	/// Shortcut management
	/// </summary>
	internal class Shortcut
	{
		/// <summary>
		/// Check if a specified shortcut file exists.
		/// </summary>
		/// <param name="shortcutPath">Shortcut file path</param>
		/// <param name="targetPath">Target file path of shortcut</param>
		/// <param name="arguments">Arguments of shortcut</param>
		/// <param name="comment">Comment of shortcut</param>
		/// <param name="workingFolder">Working folder of shortcut</param>
		/// <param name="windowState">Window state of shortcut</param>
		/// <param name="iconPath">Icon file path of shortcut</param>
		/// <param name="appId">AppUserModelID of shortcut</param>
		/// <param name="activatorId">AppUserModelToastActivatorCLSID of shortcut</param>
		/// <returns>True if exists</returns>
		public bool CheckShortcut(
			string shortcutPath,
			string targetPath,
			string arguments,
			string comment,
			string workingFolder,
			ShortcutWindowState windowState,
			string iconPath,
			string appId,
			Guid activatorId)
		{
			if (!File.Exists(shortcutPath))
				return false;

			using (var shellLink = new ShellLink(shortcutPath))
			{
				// File path casing may be different from that when installed the shortcut file.
				return (shellLink.TargetPath.IsNullOrEmptyOrEquals(targetPath, StringComparison.OrdinalIgnoreCase) &&
					shellLink.Arguments.IsNullOrEmptyOrEquals(arguments, StringComparison.Ordinal) &&
					shellLink.Description.IsNullOrEmptyOrEquals(comment, StringComparison.Ordinal) &&
					shellLink.WorkingDirectory.IsNullOrEmptyOrEquals(workingFolder, StringComparison.OrdinalIgnoreCase) &&
					(shellLink.WindowStyle == ConvertToWindowStyle(windowState)) &&
					shellLink.IconPath.IsNullOrEmptyOrEquals(iconPath, StringComparison.OrdinalIgnoreCase) &&
					shellLink.AppUserModelID.IsNullOrEmptyOrEquals(appId, StringComparison.Ordinal) &&
					((activatorId == Guid.Empty) || (shellLink.AppUserModelToastActivatorCLSID == activatorId)));
			}
		}

		/// <summary>
		/// Install a specified shortcut file.
		/// </summary>
		/// <param name="shortcutPath">Shortcut file path</param>
		/// <param name="targetPath">Target file path of shortcut</param>
		/// <param name="arguments">Arguments of shortcut</param>
		/// <param name="comment">Comment of shortcut</param>
		/// <param name="workingFolder">Working folder of shortcut</param>
		/// <param name="windowState">Window state of shortcut</param>
		/// <param name="iconPath">Icon file path of shortcut</param>
		/// <param name="appId">AppUserModelID of shortcut</param>
		/// <param name="activatorId">AppUserModelToastActivatorCLSID of shortcut</param>
		public void InstallShortcut(
			string shortcutPath,
			string targetPath,
			string arguments,
			string comment,
			string workingFolder,
			ShortcutWindowState windowState,
			string iconPath,
			string appId,
			Guid activatorId)
		{
			if (string.IsNullOrWhiteSpace(shortcutPath))
				throw new ArgumentNullException(nameof(shortcutPath));

			using (var shellLink = new ShellLink
			{
				TargetPath = targetPath,
				Arguments = arguments,
				Description = comment,
				WorkingDirectory = workingFolder,
				WindowStyle = ConvertToWindowStyle(windowState),
				IconPath = iconPath,
				IconIndex = 0, // The first icon in the file
				AppUserModelID = appId,
			})
			{
				if (activatorId != Guid.Empty)
					shellLink.AppUserModelToastActivatorCLSID = activatorId;

				shellLink.Save(shortcutPath);
			}
		}

		/// <summary>
		/// Delete a specified shortcut file.
		/// </summary>
		/// <param name="shortcutPath">Shortcut file path</param>
		/// <param name="targetPath">Target file path of shortcut</param>
		/// <param name="arguments">Arguments of shortcut</param>
		/// <param name="comment">Comment of shortcut</param>
		/// <param name="workingFolder">Working folder of shortcut</param>
		/// <param name="windowState">Window state of shortcut</param>
		/// <param name="iconPath">Icon file path of shortcut</param>
		/// <param name="appId">AppUserModelID of shortcut</param>
		/// <param name="activatorId">AppUserModelToastActivatorCLSID of shortcut</param>
		/// <remarks>If contents of shortcut do not match, the shortcut file will not be deleted.</remarks>
		public void DeleteShortcut(
			string shortcutPath,
			string targetPath,
			string arguments,
			string comment,
			string workingFolder,
			ShortcutWindowState windowState,
			string iconPath,
			string appId,
			Guid activatorId)
		{
			if (!CheckShortcut(
				shortcutPath: shortcutPath,
				targetPath: targetPath,
				arguments: arguments,
				comment: comment,
				workingFolder: workingFolder,
				windowState: windowState,
				iconPath: iconPath,
				appId: appId,
				activatorId: activatorId))
				return;

			File.Delete(shortcutPath);
		}

		#region Helper

		private static ShellLink.SW ConvertToWindowStyle(ShortcutWindowState windowState)
		{
			switch (windowState)
			{
				case ShortcutWindowState.Maximized:
					return ShellLink.SW.SW_SHOWMAXIMIZED;
				case ShortcutWindowState.Minimized:
					return ShellLink.SW.SW_SHOWMINNOACTIVE;
				default:
					return ShellLink.SW.SW_SHOWNORMAL;
			}
		}

		#endregion
	}
}