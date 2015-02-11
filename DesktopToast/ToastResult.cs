
namespace DesktopToast
{
	/// <summary>
	/// Result types of toast notifications
	/// </summary>
	public enum ToastResult
	{
		/// <summary>
		/// Toast notification is unavailable on current OS.
		/// </summary>
		Unavailable = 0,

		/// <summary>
		/// Toast request is invalid.
		/// </summary>
		Invalid,

		/// <summary>
		/// The user activated the toast.
		/// </summary>
		/// <remarks>This corresponds to ToastNotification.Activated event.</remarks>
		Activated,

		/// <summary>
		/// The application hid the toast using ToastNotifier.hide method.
		/// </summary>
		/// <remarks>This corresponds to ToastNotification.Dismissed event with ToastDismissalReason.ApplicationHidden.</remarks>
		ApplicationHidden,

		/// <summary>
		/// The user dismissed the toast.
		/// </summary>
		/// <remarks>This corresponds to ToastNotification.Dismissed event with ToastDismissalReason.UserCanceled.</remarks>
		UserCanceled,

		/// <summary>
		/// The toast has timed out.
		/// </summary>
		/// <remarks>This corresponds to ToastNotification.Dismissed event with ToastDismissalReason.TimedOut.</remarks>
		TimedOut,

		/// <summary>
		/// The toast encountered an error.
		/// </summary>
		/// <remarks>This corresponds to ToastNotification.Failed event.</remarks>
		Failed
	}
}