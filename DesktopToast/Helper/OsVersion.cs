using System;
using System.Linq;
using System.Management;

namespace DesktopToast.Helper
{
	/// <summary>
	/// OS version information
	/// </summary>
	internal class OsVersion
	{
		/// <summary>
		/// Whether OS is Windows 8 or newer
		/// </summary>
		/// <remarks>Windows 8 = version 6.2</remarks>
		public static bool IsEightOrNewer
		{
			get
			{
				if (!_isEightOrNewer.HasValue)
				{
					var ver = GetOsVersionByWmi();
					_isEightOrNewer = (ver != null) && (((6 == ver.Major) && (2 <= ver.Minor)) || (7 <= ver.Major));
				}

				return _isEightOrNewer.Value;
			}
		}
		private static bool? _isEightOrNewer;


		#region Helper

		private static Version GetOsVersionByWmi()
		{
			var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");

			var os = searcher.Get().Cast<ManagementObject>().FirstOrDefault();

			if ((os != null) && (os["OsType"] != null) && (os["Version"] != null))
			{
				if (os["OsType"].ToString() == "18") // WINNT
					return new Version(os["Version"].ToString());
			}

			return null;
		}

		#endregion
	}
}