
namespace DesktopToast
{
	/// <summary>
	/// Audio types of toast notifications
	/// </summary>
	/// <remarks>These types correspond to options in the toast audio options catalog except Silent.</remarks>
	public enum ToastAudio
	{
		/// <summary>
		/// Default
		/// </summary>
		Default = 0,

		/// <summary>
		/// Scenario: A new instant messenger notification has arrived.
		/// </summary>
		IM,

		/// <summary>
		/// Scenario: A new e-mail has arrived.
		/// </summary>
		Mail,

		/// <summary>
		/// Scenario: An calendar item is due.
		/// </summary>
		Reminder,

		/// <summary>
		/// Scenario: A new text message has arrived.
		/// </summary>
		SMS,

		#region Looping Alarm

		/// <summary>
		/// Scenario: A countdown stopwatch has reached 0.
		/// </summary>
		LoopingAlarm,

		/// <summary>
		/// Scenario: A countdown stopwatch has reached 0.
		/// </summary>
		LoopingAlarm2,

		/// <summary>
		/// Scenario: A countdown stopwatch has reached 0.
		/// </summary>
		LoopingAlarm3,

		/// <summary>
		/// Scenario: A countdown stopwatch has reached 0.
		/// </summary>
		LoopingAlarm4,

		/// <summary>
		/// Scenario: A countdown stopwatch has reached 0.
		/// </summary>
		LoopingAlarm5,

		/// <summary>
		/// Scenario: A countdown stopwatch has reached 0.
		/// </summary>
		LoopingAlarm6,

		/// <summary>
		/// Scenario: A countdown stopwatch has reached 0.
		/// </summary>
		LoopingAlarm7,

		/// <summary>
		/// Scenario: A countdown stopwatch has reached 0.
		/// </summary>
		LoopingAlarm8,

		/// <summary>
		/// Scenario: A countdown stopwatch has reached 0.
		/// </summary>
		LoopingAlarm9,

		/// <summary>
		/// Scenario: A countdown stopwatch has reached 0.
		/// </summary>
		LoopingAlarm10,

		#endregion

		#region Looping Call

		/// <summary>
		/// Scenario: An incoming phone call.
		/// </summary>
		LoopingCall,

		/// <summary>
		/// Scenario: An incoming phone call.
		/// </summary>
		LoopingCall2,

		/// <summary>
		/// Scenario: An incoming phone call.
		/// </summary>
		LoopingCall3,

		/// <summary>
		/// Scenario: An incoming phone call.
		/// </summary>
		LoopingCall4,

		/// <summary>
		/// Scenario: An incoming phone call.
		/// </summary>
		LoopingCall5,

		/// <summary>
		/// Scenario: An incoming phone call.
		/// </summary>
		LoopingCall6,

		/// <summary>
		/// Scenario: An incoming phone call.
		/// </summary>
		LoopingCall7,

		/// <summary>
		/// Scenario: An incoming phone call.
		/// </summary>
		LoopingCall8,

		/// <summary>
		/// Scenario: An incoming phone call.
		/// </summary>
		LoopingCall9,

		/// <summary>
		/// Scenario: An incoming phone call.
		/// </summary>
		LoopingCall10,

		#endregion

		/// <summary>
		/// No sound
		/// </summary>
		Silent
	}
}