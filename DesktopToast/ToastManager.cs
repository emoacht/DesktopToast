using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.UI.Notifications;

using DesktopToast.Helper;

namespace DesktopToast
{
	/// <summary>
	/// Manages toast notifications.
	/// </summary>
	public class ToastManager
	{
		/// <summary>
		/// Shows a toast.
		/// </summary>
		/// <param name="request">Toast request</param>
		/// <returns>Result of showing a toast</returns>
		public static async Task<ToastResult> ShowAsync(ToastRequest request)
		{
			if (!OsVersion.IsEightOrNewer)
				return ToastResult.Unavailable;

			if (request == null)
				throw new ArgumentNullException(nameof(request));

			if (request.IsShortcutValid)
				await CheckInstallShortcut(request);

			if (!request.IsToastValid)
				return ToastResult.Invalid;

			var document = PrepareToastDocument(request);

			return await ShowBaseAsync(document, request.AppId);
		}

		/// <summary>
		/// Shows a toast using JSON format.
		/// </summary>
		/// <param name="requestJson">Toast request in JSON format</param>
		/// <returns>Result of showing a toast</returns>
		public static async Task<ToastResult> ShowAsync(string requestJson)
		{
			ToastRequest request;
			try
			{
				request = new ToastRequest(requestJson);
			}
			catch
			{
				return ToastResult.Invalid;
			}

			return await ShowAsync(request);
		}

		/// <summary>
		/// Shows a toast without toast request.
		/// </summary>
		/// <param name="document">Toast document</param>
		/// <param name="appId">AppUserModelID</param>
		/// <returns>Result of showing a toast</returns>
		public static async Task<ToastResult> ShowAsync(XmlDocument document, string appId)
		{
			if (!OsVersion.IsEightOrNewer)
				return ToastResult.Unavailable;

			if (document == null)
				throw new ArgumentNullException(nameof(document));

			if (string.IsNullOrWhiteSpace(appId))
				throw new ArgumentNullException(nameof(appId));

			return await ShowBaseAsync(document, appId);
		}

		#region Document

		private enum AudioOption
		{
			Silent,
			Short,
			Long,
		}

		/// <summary>
		/// Prepares a toast document.
		/// </summary>
		/// <param name="request">Toast request</param>
		/// <returns>Toast document</returns>
		private static XmlDocument PrepareToastDocument(ToastRequest request)
		{
			var templateType = GetTemplateType(request);

			// Get a toast template.
			var document = ToastNotificationManager.GetTemplateContent(templateType);

			// Fill in image element.
			switch (templateType)
			{
				case ToastTemplateType.ToastImageAndText01:
				case ToastTemplateType.ToastImageAndText02:
				case ToastTemplateType.ToastImageAndText03:
				case ToastTemplateType.ToastImageAndText04:
					var imageElements = document.GetElementsByTagName("image");
					imageElements[0].Attributes.GetNamedItem("src").NodeValue = request.ToastImageFilePath;
					break;
			}

			// Fill in text elements.
			var textElements = document.GetElementsByTagName("text");
			switch (templateType)
			{
				case ToastTemplateType.ToastImageAndText01:
				case ToastTemplateType.ToastText01:
					textElements[0].AppendChild(document.CreateTextNode(request.ToastBody));
					break;

				case ToastTemplateType.ToastImageAndText02:
				case ToastTemplateType.ToastImageAndText03:
				case ToastTemplateType.ToastText02:
				case ToastTemplateType.ToastText03:
					textElements[0].AppendChild(document.CreateTextNode(request.ToastHeadline));
					textElements[1].AppendChild(document.CreateTextNode(request.ToastBody));
					break;

				case ToastTemplateType.ToastImageAndText04:
				case ToastTemplateType.ToastText04:
					textElements[0].AppendChild(document.CreateTextNode(request.ToastHeadline));
					textElements[1].AppendChild(document.CreateTextNode(request.ToastBody));
					textElements[2].AppendChild(document.CreateTextNode(request.ToastBodyExtra));
					break;
			}

			// Set audio element.
			var option = CheckAudio(request.ToastAudio);
			if (option == AudioOption.Long)
				document.DocumentElement.SetAttribute("duration", "long");

			var audioElement = document.CreateElement("audio");
			if (option == AudioOption.Silent)
			{
				audioElement.SetAttribute("silent", "true");
			}
			else
			{
				audioElement.SetAttribute("src", GetAudio(request.ToastAudio));
				audioElement.SetAttribute("loop", (option == AudioOption.Long) ? "true" : "false");
			}
			document.DocumentElement.AppendChild(audioElement);

			Debug.WriteLine(document.GetXml());

			return document;
		}

		private static ToastTemplateType GetTemplateType(ToastRequest request)
		{
			if (!string.IsNullOrWhiteSpace(request.ToastImageFilePath))
			{
				if (string.IsNullOrWhiteSpace(request.ToastHeadline))
					return ToastTemplateType.ToastImageAndText01;

				if (request.ToastHeadlineWrapsTwoLines)
					return ToastTemplateType.ToastImageAndText03;

				return string.IsNullOrWhiteSpace(request.ToastBodyExtra)
					? ToastTemplateType.ToastImageAndText02
					: ToastTemplateType.ToastImageAndText04;
			}
			else
			{
				if (string.IsNullOrWhiteSpace(request.ToastHeadline))
					return ToastTemplateType.ToastText01;

				if (request.ToastHeadlineWrapsTwoLines)
					return ToastTemplateType.ToastText03;

				return string.IsNullOrWhiteSpace(request.ToastBodyExtra)
					? ToastTemplateType.ToastText02
					: ToastTemplateType.ToastText04;
			}
		}

		private static AudioOption CheckAudio(ToastAudio audio)
		{
			switch (audio)
			{
				case ToastAudio.Silent:
					return AudioOption.Silent;

				case ToastAudio.Default:
				case ToastAudio.IM:
				case ToastAudio.Mail:
				case ToastAudio.Reminder:
				case ToastAudio.SMS:
					return AudioOption.Short;

				default:
					return AudioOption.Long;
			}
		}

		private static string GetAudio(ToastAudio audio)
		{
			return $"ms-winsoundevent:Notification.{audio.ToString().ToCamelWithSeparator('.')}";
		}

		#endregion

		#region Shortcut

		/// <summary>
		/// Waiting duration before showing a toast after the shortcut file is installed
		/// </summary>
		/// <remarks>It seems that roughly 3 seconds are required.</remarks>
		private static readonly TimeSpan _waitingDuration = TimeSpan.FromSeconds(3);

		/// <summary>
		/// Checks and installs a shortcut file in Start menu.
		/// </summary>
		/// <param name="request">Toast request</param>
		private static async Task CheckInstallShortcut(ToastRequest request)
		{
			var shortcutFilePath = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), // Not CommonStartMenu
				"Programs",
				request.ShortcutFileName);

			var shortcut = new Shortcut();

			if (!shortcut.CheckShortcut(
				shortcutPath: shortcutFilePath,
				targetPath: request.ShortcutTargetFilePath,
				arguments: request.ShortcutArguments,
				comment: request.ShortcutComment,
				workingFolder: request.ShortcutWorkingFolder,
				windowState: request.ShortcutWindowState,
				iconPath: request.ShortcutIconFilePath,
				appId: request.AppId))
			{
				shortcut.InstallShortcut(
					shortcutPath: shortcutFilePath,
					targetPath: request.ShortcutTargetFilePath,
					arguments: request.ShortcutArguments,
					comment: request.ShortcutComment,
					workingFolder: request.ShortcutWorkingFolder,
					windowState: request.ShortcutWindowState,
					iconPath: request.ShortcutIconFilePath,
					appId: request.AppId);

				await Task.Delay((TimeSpan.Zero < request.WaitingDuration) ? request.WaitingDuration : _waitingDuration);
			}
		}

		#endregion

		#region Toast

		/// <summary>
		/// Shows a toast.
		/// </summary>
		/// <param name="document">Toast document</param>
		/// <param name="appId">AppUserModelID</param>
		/// <returns>Result of showing a toast</returns>
		private static async Task<ToastResult> ShowBaseAsync(XmlDocument document, string appId)
		{
			// Create a toast and prepare to handle toast events.
			var toast = new ToastNotification(document);
			var tcs = new TaskCompletionSource<ToastResult>();

			TypedEventHandler<ToastNotification, object> activated = (sender, e) =>
			{
				tcs.SetResult(ToastResult.Activated);
			};
			toast.Activated += activated;

			TypedEventHandler<ToastNotification, ToastDismissedEventArgs> dismissed = (sender, e) =>
			{
				switch (e.Reason)
				{
					case ToastDismissalReason.ApplicationHidden:
						tcs.SetResult(ToastResult.ApplicationHidden);
						break;
					case ToastDismissalReason.UserCanceled:
						tcs.SetResult(ToastResult.UserCanceled);
						break;
					case ToastDismissalReason.TimedOut:
						tcs.SetResult(ToastResult.TimedOut);
						break;
				}
			};
			toast.Dismissed += dismissed;

			TypedEventHandler<ToastNotification, ToastFailedEventArgs> failed = (sender, e) =>
			{
				tcs.SetResult(ToastResult.Failed);
			};
			toast.Failed += failed;

			// Show a toast.
			ToastNotificationManager.CreateToastNotifier(appId).Show(toast);

			// Wait for the result.
			var result = await tcs.Task;

			Debug.WriteLine($"Toast result: {result}");

			toast.Activated -= activated;
			toast.Dismissed -= dismissed;
			toast.Failed -= failed;

			return result;
		}

		#endregion
	}
}