using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace DesktopToast.Wpf
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		public string ToastResult
		{
			get { return (string)GetValue(ToastResultProperty); }
			set { SetValue(ToastResultProperty, value); }
		}
		public static readonly DependencyProperty ToastResultProperty =
			DependencyProperty.Register(
				"ToastResult",
				typeof(string),
				typeof(MainWindow),
				new FrameworkPropertyMetadata(String.Empty));

		private async void Button_ShowToast_Click(object sender, RoutedEventArgs e)
		{
			ToastResult = await ShowToastAsync();
		}

		private async Task<string> ShowToastAsync()
		{
			var toastImageFilePath = String.Format("file:///{0}", Path.GetFullPath("Resources/toast128.png"));

			var request = new ToastRequest
			{
				ToastHeadline = "DesktopToast WPF Sample",
				ToastBody = "This is a toast test.",
				ToastImageFilePath = toastImageFilePath,
				ShortcutFileName = "DesktopToast.Wpf.lnk",
				ShortcutTargetFilePath = Assembly.GetExecutingAssembly().Location,
				AppId = "DesktopToast.Wpf",
			};

			var result = await ToastManager.ShowAsync(request);

			return result.ToString();
		}
	}
}