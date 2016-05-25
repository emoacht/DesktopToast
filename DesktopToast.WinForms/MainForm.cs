using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DesktopToast.WinForms
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
		}

		private async void Button_ShowToast_Click(object sender, EventArgs e)
		{
			TextBox_ToastResult.Text = "";

			TextBox_ToastResult.Text = await ShowToastAsync();
		}

		private async Task<string> ShowToastAsync()
		{
			var request = new ToastRequest
			{
				ToastTitle = "DesktopToast WinForms Sample",
				ToastBodyList = new[] { "This is a toast test.", "Looping sound will be played." },
				ToastAudio = DesktopToast.ToastAudio.LoopingCall,
				ShortcutFileName = "DesktopToast.WinForms.lnk",
				ShortcutTargetFilePath = Assembly.GetExecutingAssembly().Location,
				AppId = "DesktopToast.WinForms",
			};

			var result = await ToastManager.ShowAsync(request);

			return result.ToString();
		}
	}
}