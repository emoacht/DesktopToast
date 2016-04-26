using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

using DesktopToast.Helper;

namespace DesktopToast
{
	/// <summary>
	/// Base class of notification activator (for Action Center of Windows 10)
	/// </summary>
	/// <remarks>This class must not be used directly because the CLSID must be unique for each application.</remarks>
	[Guid("1EC6017A-77C7-44DB-AF97-22452FA26652"), ComVisible(true), ClassInterface(ClassInterfaceType.None)]
	[ComSourceInterfaces(typeof(INotificationActivationCallback))]
	public class NotificationActivatorBase : INotificationActivationCallback
	{
		/// <summary>
		/// Activate method to be called when a user interacts with a toast in Action Center
		/// </summary>
		/// <param name="appUserModelId">AppUserModelID of the application</param>
		/// <param name="invokedArgs">Arguments from the invoked button</param>
		/// <param name="data">Data from the input fields</param>
		/// <param name="count">The number of data elements</param>
		public void Activate(string appUserModelId, string invokedArgs, NOTIFICATION_USER_INPUT_DATA[] data, uint count)
		{
			_action?.Invoke(invokedArgs, data?.Take((int)count).ToDictionary(x => x.Key, x => x.Value));
		}

		private static int? _cookie;
		private static Action<string, Dictionary<string, string>> _action;

		/// <summary>
		/// Register COM class type.
		/// </summary>
		/// <param name="activatorType">Notification activator type</param>
		/// <param name="action">Action to be invoked when Activate callback method is called</param>
		/// <remarks>Notification activator must inherit from this class.</remarks>
		public static void RegisterComType(Type activatorType, Action<string, Dictionary<string, string>> action)
		{
			NotificationHelper.CheckArgument(activatorType);

			if (!OsVersion.IsTenOrNewer)
				return;

			if (_cookie.HasValue)
				return;

			_cookie = new RegistrationServices().RegisterTypeForComClients(
				activatorType,
				RegistrationClassContext.LocalServer,
				RegistrationConnectionType.MultipleUse);

			_action = action;
		}

		/// <summary>
		/// Unregister COM class type.
		/// </summary>
		public static void UnregisterComType()
		{
			if (!_cookie.HasValue)
				return;

			new RegistrationServices().UnregisterTypeForComClients(_cookie.Value);
			_cookie = null;
			_action = null;
		}
	}

	/// <summary>
	/// INotificationActivationCallback Interface
	/// </summary>
	[ComImport, Guid("53E31837-6600-4A81-9395-75CFFE746F94"), ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface INotificationActivationCallback
	{
		/// <summary>
		/// Activate method to be called when a user interacts with a toast in Action Center
		/// </summary>
		/// <param name="appUserModelId">AppUserModelID of the application</param>
		/// <param name="invokedArgs">Arguments from the invoked button</param>
		/// <param name="data">Data from the input fields</param>
		/// <param name="count">The number of data elements</param>
		void Activate(
			[MarshalAs(UnmanagedType.LPWStr)] string appUserModelId,
			[MarshalAs(UnmanagedType.LPWStr)] string invokedArgs,
			[MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] NOTIFICATION_USER_INPUT_DATA[] data,
			uint count);
	}

	/// <summary>
	/// NOTIFICATION_USER_INPUT_DATA Structure
	/// </summary>
	[StructLayout(LayoutKind.Sequential), Serializable]
	public struct NOTIFICATION_USER_INPUT_DATA
	{
		/// <summary>
		/// ID of the user input field
		/// </summary>
		[MarshalAs(UnmanagedType.LPWStr)]
		public string Key;

		/// <summary>
		/// Input value selected by a user for the given input field
		/// </summary>
		[MarshalAs(UnmanagedType.LPWStr)]
		public string Value;
	}
}