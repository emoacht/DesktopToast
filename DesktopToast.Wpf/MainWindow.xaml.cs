using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using NotificationsExtensions;
using NotificationsExtensions.Toasts;

namespace DesktopToast.Wpf
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			// If "-Embedding" argument is appended, it will mean this application is started by COM.
			if (Environment.GetCommandLineArgs().Last() == "-Embedding")
				this.Title += " [COM]";
		}

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);

			// For Action Center of Windows 10
			NotificationActivator.RegisterComType(typeof(NotificationActivator), OnActivated);

			NotificationHelper.RegisterComServer(typeof(NotificationActivator), Assembly.GetExecutingAssembly().Location);
			//NotificationHelper.UnregisterComServer(typeof(NotificationActivator));
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);

			// For Action Center of Windows 10
			NotificationActivator.UnregisterComType();
		}

		private const string MessageId = "Message";

		private void OnActivated(string arguments, Dictionary<string, string> data)
		{
			var result = "Activated";
			if ((arguments?.StartsWith("action=")).GetValueOrDefault())
			{
				result = arguments.Substring("action=".Length);

				if ((data?.ContainsKey(MessageId)).GetValueOrDefault())
					Dispatcher.Invoke(() => Message = data[MessageId]);
			}
			Dispatcher.Invoke(() => ActivationResult = result);
		}

		#region Property

		public string ToastResult
		{
			get { return (string)GetValue(ToastResultProperty); }
			set { SetValue(ToastResultProperty, value); }
		}
		public static readonly DependencyProperty ToastResultProperty =
			DependencyProperty.Register(
				nameof(ToastResult),
				typeof(string),
				typeof(MainWindow),
				new PropertyMetadata(string.Empty));

		public string ActivationResult
		{
			get { return (string)GetValue(ActivationResultProperty); }
			set { SetValue(ActivationResultProperty, value); }
		}
		public static readonly DependencyProperty ActivationResultProperty =
			DependencyProperty.Register(
				nameof(ActivationResult),
				typeof(string),
				typeof(MainWindow),
				new PropertyMetadata(string.Empty));

		public string Message
		{
			get { return (string)GetValue(MessageProperty); }
			set { SetValue(MessageProperty, value); }
		}
		public static readonly DependencyProperty MessageProperty =
			DependencyProperty.Register(
				nameof(Message),
				typeof(string),
				typeof(MainWindow),
				new PropertyMetadata(string.Empty));

		public bool CanUseInteractiveToast
		{
			get { return (bool)GetValue(CanUseInteractiveToastProperty); }
			set { SetValue(CanUseInteractiveToastProperty, value); }
		}
		public static readonly DependencyProperty CanUseInteractiveToastProperty =
			DependencyProperty.Register(
				nameof(CanUseInteractiveToast),
				typeof(bool),
				typeof(MainWindow),
				new PropertyMetadata(Environment.OSVersion.Version.Major >= 10));

		#endregion

		private void Clear()
		{
			ToastResult = "";
			ActivationResult = "";
			Message = "";
		}

		private async void Button_ShowToast_Click(object sender, RoutedEventArgs e)
		{
			Clear();

			ToastResult = await ShowToastAsync();
		}

		private async Task<string> ShowToastAsync()
		{
			var request = new ToastRequest
			{
				ToastTitle = "DesktopToast WPF Sample",
				ToastBody = "This is a toast test.",
				ToastLogoFilePath = string.Format("file:///{0}", Path.GetFullPath("Resources/toast128.png")),
				ShortcutFileName = "DesktopToast.Wpf.lnk",
				ShortcutTargetFilePath = Assembly.GetExecutingAssembly().Location,
				AppId = "DesktopToast.Wpf",
				ActivatorId = typeof(NotificationActivator).GUID // For Action Center of Windows 10
			};

			var result = await ToastManager.ShowAsync(request);

			return result.ToString();
		}

		private async void Button_ShowInteractiveToast_Click(object sender, RoutedEventArgs e)
		{
			Clear();

			ToastResult = await ShowInteractiveToastAsync();
		}

		private async Task<string> ShowInteractiveToastAsync()
		{
			var request = new ToastRequest
			{
				ToastXml = ComposeInteractiveToast(),
				ShortcutFileName = "DesktopToast.Wpf.lnk",
				ShortcutTargetFilePath = Assembly.GetExecutingAssembly().Location,
				AppId = "DesktopToast.Wpf",
				ActivatorId = typeof(NotificationActivator).GUID
			};

			var result = await ToastManager.ShowAsync(request);

			return result.ToString();
		}

		private string ComposeInteractiveToast()
		{
			var toastVisual = new ToastVisual
			{
				BindingGeneric = new ToastBindingGeneric
				{
					Children =
					{
						new AdaptiveText { Text = "DesktopToast WPF Sample" }, // Title
						new AdaptiveText { Text = "This is an interactive toast test." }, // Body
					},
					AppLogoOverride = new ToastGenericAppLogo
					{
						Source = string.Format("file:///{0}", Path.GetFullPath("Resources/toast128.png")),
						AlternateText = "Logo"
					}
				}
			};
			var toastAction = new ToastActionsCustom
			{
				Inputs =
				{
					new ToastTextBox(id: MessageId) { PlaceholderContent = "Input a message" }
				},
				Buttons =
				{
					new ToastButton(content: "Reply", arguments: "action=Replied") { ActivationType = ToastActivationType.Background },
					new ToastButton(content: "Ignore", arguments: "action=Ignored")
				}
			};
			var toastContent = new ToastContent
			{
				Visual = toastVisual,
				Actions = toastAction,
				Duration = ToastDuration.Long,
				Audio = new NotificationsExtensions.Toasts.ToastAudio
				{
					Loop = true,
					Src = new Uri("ms-winsoundevent:Notification.Looping.Alarm4")
				}
			};

			return toastContent.GetContent();
		}
	}
}