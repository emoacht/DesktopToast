using System.Runtime.InteropServices;

namespace DesktopToast.Helper
{
    /// <summary>
    /// OS version information
    /// </summary>
    internal class OsVersion
    {
        #region Win32

        [DllImport("kernel32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool VerifyVersionInfoW(
            [In] ref OSVERSIONINFOEX lpVersionInfo,
            uint dwTypeMask,
            ulong dwlConditionMask);

        [DllImport("kernel32", SetLastError = true)]
        private static extern ulong VerSetConditionMask(
            ulong dwlConditionMask,
            uint dwTypeBitMask,
            byte dwConditionMask);

        [StructLayout(LayoutKind.Sequential)]
        private struct OSVERSIONINFOEX
        {
            public uint dwOSVersionInfoSize;
            public uint dwMajorVersion;
            public uint dwMinorVersion;
            public uint dwBuildNumber;
            public uint dwPlatformId;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szCSDVersion;
            public ushort wServicePackMajor;
            public ushort wServicePackMinor;
            public ushort wSuiteMask;
            public byte wProductType;
            public byte wReserved;
        }

        private static uint VER_MINORVERSION = 0x0000001;
        private static uint VER_MAJORVERSION = 0x0000002;

        private static byte VER_GREATER_EQUAL = 3;

        #endregion


        /// <summary>
        /// Whether OS is Windows 8 or newer
        /// </summary>
        /// <remarks>Windows 8 = version 6.2</remarks>
        public static bool IsEightOrNewer
        {
            get
            {
                if (!_isEightOrNewer.HasValue)
                    _isEightOrNewer = IsEqualOrNewerByVerifyVersionInfo(6, 2);

                return _isEightOrNewer.HasValue ? _isEightOrNewer.Value : false;
            }
        }
        private static bool? _isEightOrNewer;

        private static bool? IsEqualOrNewerByVerifyVersionInfo(int major, int minor)
        {
            var info = new OSVERSIONINFOEX();
            info.dwMajorVersion = (uint)major;
            info.dwMinorVersion = (uint)minor;
            info.dwOSVersionInfoSize = (uint)Marshal.SizeOf(info);

            ulong cm = 0;
            cm = VerSetConditionMask(cm, VER_MAJORVERSION, VER_GREATER_EQUAL);
            cm = VerSetConditionMask(cm, VER_MINORVERSION, VER_GREATER_EQUAL);

            var result = VerifyVersionInfoW(ref info, VER_MAJORVERSION | VER_MINORVERSION, cm);

            if (result)
                return true;

            return (Marshal.GetLastWin32Error() == 1150) // ERROR_OLD_WIN_VERSION
                ? false
                : (bool?)null;
        }
    }
}