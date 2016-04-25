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

namespace DesktopToast.Wpf
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			// If "-Embedding" argument is appended, it means that this application is started by COM.
			if (Environment.GetCommandLineArgs().Last() == "-Embedding")
				this.Title += " [COM]";
		}

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);

			// For Action Center of Windows 10
			NotificationActivator.RegisterComObject(typeof(NotificationActivator), OnActivated);

			NotificationHelper.RegisterComServer(typeof(NotificationActivator), Assembly.GetExecutingAssembly().Location);
			//NotificationHelper.UnregisterComServer(typeof(NotificationActivator));
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);

			// For Action Center of Windows 10
			NotificationActivator.UnregisterComObject();
		}

		private void OnActivated(string arguments, Dictionary<string, string> data)
		{
			Dispatcher.Invoke(() => ActivationResult = $"Activated {arguments}");
		}

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

		private async void Button_ShowToast_Click(object sender, RoutedEventArgs e)
		{
			ToastResult = "";
			ActivationResult = "";

			ToastResult = await ShowToastAsync();
		}

		private async Task<string> ShowToastAsync()
		{
			var request = new ToastRequest
			{
				ToastHeadline = "DesktopToast WPF Sample",
				ToastBody = "This is a toast test.",
				ToastImageFilePath = string.Format("file:///{0}", Path.GetFullPath("Resources/toast128.png")),
				ShortcutFileName = "DesktopToast.Wpf.lnk",
				ShortcutTargetFilePath = Assembly.GetExecutingAssembly().Location,
				AppId = "DesktopToast.Wpf",
				ActivatorId = typeof(NotificationActivator).GUID // For Action Center of Windows 10
			};

			var result = await ToastManager.ShowAsync(request);

			return result.ToString();
		}
	}
}