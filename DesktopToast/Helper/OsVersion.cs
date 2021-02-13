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
				return _isEightOrNewer.Value;
			}
		}
		private static readonly Lazy<bool> _isEightOrNewer = new Lazy<bool>(GetIsEightOrNewer);

		private static bool GetIsEightOrNewer()
		{
			return (Ver != null) && (((6 == Ver.Major) && (2 <= Ver.Minor)) || (7 <= Ver.Major));
		}

		/// <summary>
		/// Whether OS is Windows 10 or newer
		/// </summary>
		/// <remarks>Windows 10 = version 10.0.10240.0 or higher</remarks>
		public static bool IsTenOrNewer
		{
			get
			{
				return _isTenOrNewer.Value;
			}
		}
		private static readonly Lazy<bool> _isTenOrNewer = new Lazy<bool>(GetIsTenOrNewer);

		private static bool GetIsTenOrNewer()
		{
			return (Ver != null) && (10 <= Ver.Major);
		}

		#region Helper

		/// <summary>
		/// OS version.
		/// </summary>
		private static Version Ver
		{
			get
			{
				return _ver.Value;
			}
		}
		private static readonly Lazy<Version> _ver = new Lazy<Version>(GetOsVersionByWmi);

		/// <summary>
		/// Get OS version.
		/// </summary>
		/// <returns>OS version</returns>
		/// <remarks>Even on Windows 8.1 or newer, WMI seems not affected by the application manifest and so
		/// always returns correct version number.</remarks>
		private static Version GetOsVersionByWmi()
		{
			using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem"))
			{
				var os = searcher.Get().Cast<ManagementObject>().FirstOrDefault();

				var osTypeValue = (ushort)(os?["OSType"] ?? 0);
				if (osTypeValue == 18) // WINNT
				{
					var versionValue = os?["Version"] as string;
					if (versionValue != null)
						return new Version(versionValue);
				}

				return null;
			}
		}

		#endregion
	}
}