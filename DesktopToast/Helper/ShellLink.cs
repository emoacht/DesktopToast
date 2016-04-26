using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

using ComTypes = System.Runtime.InteropServices.ComTypes;

namespace DesktopToast.Helper
{
	/// <summary>
	/// A wrapper class for IShellLink Interface added with AppUserModelID and AppUserModelToastActivatorCLSID
	/// </summary>
	/// <remarks>
	/// Modified from http://smdn.jp/programming/tips/createlnk/
	/// Originally from http://www.vbaccelerator.com/home/NET/Code/Libraries/Shell_Projects/Creating_and_Modifying_Shortcuts/article.asp
	/// </remarks>
	internal class ShellLink : IDisposable
	{
		#region Win32 and COM

		/// <summary>
		/// IShellLink Interface
		/// </summary>
		[ComImport, Guid("000214F9-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		private interface IShellLinkW
		{
			uint GetPath([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile,
						 int cchMaxPath,
						 ref WIN32_FIND_DATAW pfd,
						 SLGP fFlags);

			uint GetIDList(out IntPtr ppidl);
			uint SetIDList(IntPtr pidl);

			uint GetDescription([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszName,
								int cchMaxName);

			uint SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);

			uint GetWorkingDirectory([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir,
									 int cchMaxPath);

			uint SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);

			uint GetArguments([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs,
							  int cchMaxPath);

			uint SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);
			uint GetHotKey(out ushort pwHotkey);
			uint SetHotKey(ushort wHotKey);
			uint GetShowCmd(out SW piShowCmd);
			uint SetShowCmd(SW iShowCmd);

			uint GetIconLocation([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath,
								 int cchIconPath,
								 out int piIcon);

			uint SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath,
								 int iIcon);

			uint SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel,
								 uint dwReserved);

			uint Resolve(IntPtr hwnd, uint fFlags);
			uint SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
		}

		/// <summary>
		/// ShellLink CoClass (Shell link object)
		/// </summary>
		[ComImport, Guid("00021401-0000-0000-C000-000000000046"), ClassInterface(ClassInterfaceType.None)]
		private class CShellLink
		{ }

		/// <summary>
		/// WIN32_FIND_DATAW Structure
		/// </summary>
		[StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode), Serializable]
		private struct WIN32_FIND_DATAW
		{
			public uint dwFileAttributes;
			public ComTypes.FILETIME ftCreationTime;
			public ComTypes.FILETIME ftLastAccessTime;
			public ComTypes.FILETIME ftLastWriteTime;
			public uint nFileSizeHigh;
			public uint nFileSizeLow;
			public uint dwReserved0;
			public uint dwReserved1;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
			public string cFileName;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
			public string cAlternateFileName;
		}

		/// <summary>
		/// IPropertyStore Interface
		/// </summary>
		[ComImport, Guid("886D8EEB-8CF2-4446-8D02-CDBA1DBDCF99"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		private interface IPropertyStore
		{
			uint GetCount([Out] out uint cProps);

			uint GetAt([In] uint iProp,
					   out PropertyKey pkey);

			uint GetValue([In] ref PropertyKey key,
						  [Out] PropVariant pv);

			uint SetValue([In] ref PropertyKey key,
						  [In] PropVariant pv);

			uint Commit();
		}

		/// <summary>
		/// PropertyKey Structure
		/// </summary>
		/// <remarks>
		/// Narrowed down from PropertyKey.cs of Windows API Code Pack 1.1
		/// </remarks>
		[StructLayout(LayoutKind.Sequential, Pack = 4)]
		private struct PropertyKey
		{
			#region Field

			private Guid formatId; // Unique GUID for property
			private int propertyId; // Property identifier (PID)

			#endregion

			#region Property

			public Guid FormatId => this.formatId;
			public int PropertyId => this.propertyId;

			#endregion

			#region Constructor

			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="formatId">Format ID</param>
			/// <param name="propertyId">Property ID</param>
			public PropertyKey(string formatId, int propertyId)
			{
				this.formatId = new Guid(formatId);
				this.propertyId = propertyId;
			}

			#endregion
		}

		/// <summary>
		/// PropVariant Class (only for limited types)
		/// </summary>
		/// <remarks>
		/// Narrowed down from PropVariant.cs of Windows API Code Pack 1.1
		/// Originally from https://blogs.msdn.microsoft.com/adamroot/2008/04/11/interop-with-propvariants-in-net/
		/// </remarks>
		[StructLayout(LayoutKind.Explicit)]
		private sealed class PropVariant : IDisposable
		{
			#region Field

			[FieldOffset(0)]
			private ushort valueType;

			// [FieldOffset(2)]
			// private ushort wReserved1;
			// [FieldOffset(4)]
			// private ushort wReserved2;
			// [FieldOffset(6)]
			// private ushort wReserved3;

			[FieldOffset(8)]
			private IntPtr value;

			#endregion

			#region Property

			/// <summary>
			/// Value type (System.Runtime.InteropServices.VarEnum)
			/// </summary>
			public VarEnum ValueType
			{
				get { return (VarEnum)this.valueType; }
				set { this.valueType = (ushort)value; }
			}

			/// <summary>
			/// Whether value is empty or null
			/// </summary>
			public bool IsNullOrEmpty =>
				(this.valueType == (ushort)VarEnum.VT_EMPTY) ||
				(this.valueType == (ushort)VarEnum.VT_NULL);

			/// <summary>
			/// Value (only for limited types)
			/// </summary>
			public object Value
			{
				get
				{
					switch ((VarEnum)this.valueType)
					{
						case VarEnum.VT_LPWSTR:
							return Marshal.PtrToStringUni(this.value);
						case VarEnum.VT_CLSID:
							return Marshal.PtrToStructure<Guid>(this.value);
						default: // VT_EMPTY and so on
							return null;
					}
				}
			}

			#endregion

			#region Constructor

			public PropVariant()
			{ }

			/// <summary>
			/// Constructor with string value
			/// </summary>
			/// <param name="value">String value</param>
			public PropVariant(string value)
			{
				if (value == null)
					throw new ArgumentNullException(nameof(value));

				this.valueType = (ushort)VarEnum.VT_LPWSTR;
				this.value = Marshal.StringToCoTaskMemUni(value);
			}

			/// <summary>
			/// Constructor with CLSID value
			/// </summary>
			/// <param name="value">CLSID value</param>
			public PropVariant(Guid value)
			{
				if (value == Guid.Empty)
					throw new ArgumentNullException(nameof(value));

				this.valueType = (ushort)VarEnum.VT_CLSID;
				this.value = Marshal.AllocCoTaskMem(Marshal.SizeOf(value));
				Marshal.StructureToPtr(value, this.value, false);
			}

			#endregion

			#region Destructor

			~PropVariant()
			{
				Dispose();
			}

			public void Dispose()
			{
				PropVariantClear(this);
				GC.SuppressFinalize(this);
			}

			#endregion
		}

		[DllImport("Ole32.dll", PreserveSig = false)]
		private extern static void PropVariantClear([In, Out] PropVariant pvar); // Or ref

		/// <summary>
		/// Property key of Arguments
		/// </summary>
		/// <remarks>
		/// Name = System.Link.Arguments
		/// ShellPKey = PKEY_Link_Arguments
		/// FormatID = 436F2667-14E2-4FEB-B30A-146C53B5B674
		/// PropID = 100
		/// Type = String (VT_LPWSTR)
		/// </remarks>
		private static readonly PropertyKey argumentsKey = new PropertyKey("{436F2667-14E2-4FEB-B30A-146C53B5B674}", 100);

		/// <summary>
		/// Property key of AppUserModelID
		/// </summary>
		/// <remarks>
		/// Name = System.AppUserModel.ID
		/// ShellPKey = PKEY_AppUserModel_ID
		/// FormatID = 9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3
		/// PropID = 5
		/// Type = String (VT_LPWSTR)
		/// </remarks>
		private static readonly PropertyKey appUserModelIDKey = new PropertyKey("{9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3}", 5);

		/// <summary>
		/// Property key of AppUserModelToastActivatorCLSID
		/// </summary>
		/// <remarks>
		/// Name = System.AppUserModel.ToastActivatorCLSID
		/// ShellPKey = PKEY_AppUserModel_ToastActivatorCLSID
		/// FormatID = 9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3
		/// PropID = 26
		/// Type = Guid (VT_CLSID)
		/// Taken from propkey.h of Windows SDK
		/// </remarks>
		private static readonly PropertyKey appUserModelToastActivatorCLSIDKey = new PropertyKey("{9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3}", 26);

		/// <summary>
		/// STGM Constants
		/// </summary>
		internal enum STGM : int
		{
			STGM_READ = 0x00000000,
			STGM_WRITE = 0x00000001,
			STGM_READWRITE = 0x00000002,
			STGM_SHARE_DENY_NONE = 0x00000040,
			STGM_SHARE_DENY_READ = 0x00000030,
			STGM_SHARE_DENY_WRITE = 0x00000020,
			STGM_SHARE_EXCLUSIVE = 0x00000010,
			STGM_PRIORITY = 0x00040000,
			STGM_CREATE = 0x00001000,
			STGM_CONVERT = 0x00020000,
			STGM_FAILIFTHERE = 0x00000000,
			STGM_DIRECT = 0x00000000,
			STGM_TRANSACTED = 0x00010000,
			STGM_NOSCRATCH = 0x00100000,
			STGM_NOSNAPSHOT = 0x00200000,
			STGM_SIMPLE = 0x08000000,
			STGM_DIRECT_SWMR = 0x00400000,
			STGM_DELETEONRELEASE = 0x04000000
		}

		/// <summary>
		/// SLGP Flags
		/// </summary>
		internal enum SLGP : uint
		{
			SLGP_SHORTPATH = 0x1,
			SLGP_UNCPRIORITY = 0x2,
			SLGP_RAWPATH = 0x4,
			SLGP_RELATIVEPRIORITY = 0x8
		}

		/// <summary>
		/// SW (ShowWindow command)
		/// </summary>
		internal enum SW : int
		{
			SW_SHOWNORMAL = 1,
			SW_SHOWMAXIMIZED = 3,
			SW_SHOWMINNOACTIVE = 7,
		}

		#endregion

		#region Field

		/// <summary>
		/// Shell link object
		/// </summary>
		private IShellLinkW shellLink;

		/// <summary>
		/// Maximum path length limitation
		/// </summary>
		private const int MAX_PATH = 260;

		#endregion

		#region Private Property

		private IPersistFile PersistFile
		{
			get
			{
				var pf = this.shellLink as IPersistFile;
				if (pf == null)
					throw new COMException("Failed to create IPersistFile.");

				return pf;
			}
		}

		private IPropertyStore PropertyStore
		{
			get
			{
				var ps = this.shellLink as IPropertyStore;
				if (ps == null)
					throw new COMException("Failed to create IPropertyStore.");

				return ps;
			}
		}

		#endregion

		#region Internal Property (not exhaustive)

		/// <summary>
		/// Shortcut file path
		/// </summary>
		internal string ShortcutPath
		{
			get
			{
				string buff;
				this.PersistFile.GetCurFile(out buff);

				return buff;
			}
		}

		/// <summary>
		/// Target file path
		/// </summary>
		/// <remarks>This length is limited to maximum path length limitation (260) - last null (1).</remarks>
		internal string TargetPath
		{
			get
			{
				var sb = new StringBuilder(MAX_PATH - 1);
				var data = new WIN32_FIND_DATAW();
				VerifySucceeded(this.shellLink.GetPath(sb, sb.Capacity, ref data, SLGP.SLGP_UNCPRIORITY));

				return sb.ToString();
			}
			set
			{
				if ((value != null) && (MAX_PATH - 1 < value.Length))
					throw new ArgumentException("Target file path is too long.", nameof(TargetPath));

				VerifySucceeded(this.shellLink.SetPath(value));
			}
		}

		/// <summary>
		/// Arguments
		/// </summary>
		/// <remarks>
		/// <para>According to MSDN, this length should not have a limitation as long as it in Unicode.
		/// In addition, it is recommended to retrieve argument strings though IPropertyStore rather than
		/// GetArguments method.</para>
		/// <para>The setter accepts Null while the getter never returns Null. This behavior is the same
		/// as other properties by IShellLink.</para>
		/// </remarks>
		internal string Arguments
		{
			get
			{
				using (var pv = new PropVariant())
				{
					VerifySucceeded(this.PropertyStore.GetValue(argumentsKey, pv));

					return (pv.Value as string) ?? string.Empty;
				}
			}
			set { VerifySucceeded(this.shellLink.SetArguments(value)); }
		}

		/// <summary>
		/// Description
		/// </summary>
		/// <remarks>According to MSDN, this length is limited to INFOTIPSIZE. However, in practice,
		/// there seems to be the same limitation as the maximum path length limitation. Moreover,
		/// Description longer than the limitation will screw up arguments.</remarks>
		internal string Description
		{
			get
			{
				var sb = new StringBuilder(MAX_PATH);
				VerifySucceeded(this.shellLink.GetDescription(sb, sb.Capacity));

				return sb.ToString();
			}
			set
			{
				if ((value != null) && (MAX_PATH < value.Length))
					throw new ArgumentException("Description is too long.", nameof(Description));

				VerifySucceeded(this.shellLink.SetDescription(value));
			}
		}

		/// <summary>
		/// Working directory
		/// </summary>
		/// <remarks>This length is limited to maximum path length limitation (260) - last null (1).</remarks>
		internal string WorkingDirectory
		{
			get
			{
				var sb = new StringBuilder(MAX_PATH - 1);
				VerifySucceeded(this.shellLink.GetWorkingDirectory(sb, sb.Capacity));

				return sb.ToString();
			}
			set
			{
				if ((value != null) && (MAX_PATH - 1 < value.Length))
					throw new ArgumentException("Working directory is too long.", nameof(WorkingDirectory));

				VerifySucceeded(this.shellLink.SetWorkingDirectory(value));
			}
		}

		/// <summary>
		/// Window style
		/// </summary>
		internal SW WindowStyle
		{
			get
			{
				SW showCmd;
				VerifySucceeded(this.shellLink.GetShowCmd(out showCmd));

				return showCmd;
			}
			set { VerifySucceeded(this.shellLink.SetShowCmd(value)); }
		}

		/// <summary>
		/// Shortcut icon file path (Path element of icon location)
		/// </summary>
		/// <remarks>This length is limited to the maximum path length limitation (260) - last null (1).</remarks>
		internal string IconPath
		{
			get
			{
				var sb = new StringBuilder(MAX_PATH - 1);
				int index;
				VerifySucceeded(this.shellLink.GetIconLocation(sb, sb.Capacity, out index));

				return sb.ToString();
			}
			set
			{
				if ((value != null) && (MAX_PATH - 1 < value.Length))
					throw new ArgumentException("Shortcut icon file path is too long.", nameof(IconPath));

				VerifySucceeded(this.shellLink.SetIconLocation(value, IconIndex));
			}
		}

		/// <summary>
		/// Shortcut icon index (Index element of icon location)
		/// </summary>
		internal int IconIndex
		{
			get
			{
				var sb = new StringBuilder(MAX_PATH);
				int index;
				VerifySucceeded(this.shellLink.GetIconLocation(sb, sb.Capacity, out index));

				return index;
			}
			set
			{
				int index = (0 <= value) ? value : 0;
				VerifySucceeded(this.shellLink.SetIconLocation(IconPath, index));
			}
		}

		/// <summary>
		/// AppUserModelID (to be used for Windows 7 or newer)
		/// </summary>
		/// <remarks>
		/// <para>According to MSDN, an AppUserModelID must be in the following form:
		/// CompanyName.ProductName.SubProduct.VersionInformation
		/// It can have no more than 128 characters and cannot contain spaces. Each section should be
		/// camel-cased. CompanyName and ProductName should always be used, while SubProduct and
		/// VersionInformation are optional.</para>
		/// <para>The setter accepts Null while the getter never returns Null. This behavior is the same
		/// as other properties by IShellLink.</para>
		/// </remarks>
		internal string AppUserModelID
		{
			get
			{
				using (var pv = new PropVariant())
				{
					VerifySucceeded(this.PropertyStore.GetValue(appUserModelIDKey, pv));

					return (pv.Value as string) ?? string.Empty;
				}
			}
			set
			{
				var buff = value ?? string.Empty;
				if (128 < buff.Length)
					throw new ArgumentException("AppUserModelID is too long.", nameof(AppUserModelID));

				using (var pv = new PropVariant(buff))
				{
					VerifySucceeded(this.PropertyStore.SetValue(appUserModelIDKey, pv));
					VerifySucceeded(this.PropertyStore.Commit());
				}
			}
		}

		/// <summary>
		/// AppUserModelToastActivatorCLSID (to be used for Windows 10 or newer)
		/// </summary>
		public Guid AppUserModelToastActivatorCLSID
		{
			get
			{
				using (var pv = new PropVariant())
				{
					VerifySucceeded(this.PropertyStore.GetValue(appUserModelToastActivatorCLSIDKey, pv));

					return (pv.Value is Guid) ? (Guid)pv.Value : Guid.Empty;
				}
			}
			set
			{
				using (var pv = new PropVariant(value))
				{
					VerifySucceeded(this.PropertyStore.SetValue(appUserModelToastActivatorCLSIDKey, pv));
					VerifySucceeded(this.PropertyStore.Commit());
				}
			}
		}

		#endregion

		#region Constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public ShellLink() : this(null)
		{ }

		/// <summary>
		/// Constructor with creating shell link object and loading shortcut file
		/// </summary>
		/// <param name="shortcutPath">Shortcut file path</param>
		public ShellLink(string shortcutPath)
		{
			try
			{
				shellLink = (IShellLinkW)new CShellLink();
			}
			catch (Exception ex)
			{
				throw new COMException("Failed to create Shell link object.", ex);
			}

			if (shortcutPath != null) // To avoid default constructor
				this.Load(shortcutPath);
		}

		#endregion

		#region Destructor

		~ShellLink()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (shellLink != null)
			{
				// Release all references.
				Marshal.FinalReleaseComObject(shellLink);
				shellLink = null;
			}
		}

		#endregion

		#region Load/Save

		/// <summary>
		/// Load shortcut file.
		/// </summary>
		/// <param name="shortcutPath">Shortcut file path</param>
		internal void Load(string shortcutPath)
		{
			if (string.IsNullOrWhiteSpace(shortcutPath))
				throw new ArgumentNullException(nameof(shortcutPath));

			if (!File.Exists(shortcutPath))
				throw new FileNotFoundException("Shortcut file is not found.", shortcutPath);

			this.PersistFile.Load(shortcutPath, (int)STGM.STGM_READ);
		}

		/// <summary>
		/// Save shortcut file.
		/// </summary>
		internal void Save()
		{
			this.Save(this.ShortcutPath);
		}

		/// <summary>
		/// Save shortcut file.
		/// </summary>
		/// <param name="shortcutPath">Shortcut file path</param>
		internal void Save(string shortcutPath)
		{
			if (string.IsNullOrWhiteSpace(shortcutPath))
				throw new ArgumentNullException(nameof(shortcutPath));

			this.PersistFile.Save(shortcutPath, true);
		}

		#endregion

		#region Helper

		/// <summary>
		/// Verify if operation succeeded.
		/// </summary>
		/// <param name="hresult">HRESULT</param>
		/// <remarks>This method is from Sending toast notifications from desktop apps sample.</remarks>
		private void VerifySucceeded(uint hresult)
		{
			if (hresult > 1)
				throw new Exception("Failed with HRESULT: " + hresult.ToString("X"));
		}

		#endregion
	}
}