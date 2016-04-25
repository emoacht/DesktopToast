using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DesktopToast.Wpf
{
	/// <summary>
	/// Inherited class of notification activator (for Action Center of Windows 10)
	/// </summary>
	/// <remarks>The CLSID of this class must be unique for each application.</remarks>
	[Guid("f5b13fa4-8472-4f82-8a47-515b879006ba"), ComVisible(true), ClassInterface(ClassInterfaceType.None)]
	[ComSourceInterfaces(typeof(INotificationActivationCallback))]
	public class NotificationActivator : NotificationActivatorBase
	{ }
}