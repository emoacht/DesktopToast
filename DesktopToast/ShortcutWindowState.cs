
namespace DesktopToast
{
	/// <summary>
	/// Window states of shortcut
	/// </summary>
	/// <remarks>
	/// These states are equivalent to:
	/// System.Windows.WindowState enumeration in WPF
	/// System.Windows.Forms.FormWindowState enumeration in WinForms
	/// WindowStyle values in WshShortcut
	/// ShowWindow commands in IShellLink
	/// </remarks>
	public enum ShortcutWindowState
	{
		/// <summary>
		/// Activates and displays a window. If the window is minimized or maximized, the system restores
		/// it to its original size and position.
		/// </summary>
		Normal = 0,

		/// <summary>
		/// Activates the window and displays it as a maximized window.
		/// </summary>
		Maximized,

		/// <summary>
		/// Displays the window in its minimized state, leaving the currently active window as active.
		/// </summary>
		Minimized
	}
}