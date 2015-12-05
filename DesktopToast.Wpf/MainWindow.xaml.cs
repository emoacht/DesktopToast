using System;
using System.Collections.Generic;
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
				new PropertyMetadata(string.Empty));

		private async void Button_ShowToast_Click(object sender, RoutedEventArgs e)
		{
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
			};

			var result = await ToastManager.ShowAsync(request);

			return result.ToString();
		}
	}
}