using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace LanguageWorkbench
{
	public static class UIHelper
	{
		public static void SetTimeout(int milliseconds, Action func)
		{
			var timer = new DispatcherTimerContainingAction
			{
				Interval = new TimeSpan(0, 0, 0, 0, milliseconds),
				Action = func
			};
			timer.Tick += _onTimeout;
			timer.Start();
		}

		private static void _onTimeout(object sender, EventArgs arg)
		{
			var t = sender as DispatcherTimerContainingAction;
			t.Stop();
			t.Action();
			t.Tick -= _onTimeout;
		}
	}

	public class DispatcherTimerContainingAction : System.Windows.Threading.DispatcherTimer
	{
		/// <summary>
		/// uncomment this to see when the DispatcherTimerContainingAction is collected
		/// if you remove  t.Tick -= _onTimeout; line from _onTimeout method
		/// you will see that the timer is never collected
		/// </summary>
		//~DispatcherTimerContainingAction()
		//{
		//    throw new Exception("DispatcherTimerContainingAction is disposed");
		//}

		public Action Action { get; set; }
	}
}
