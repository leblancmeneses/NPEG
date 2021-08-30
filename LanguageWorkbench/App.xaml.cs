using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Browser;
using Telerik.Windows.Controls;

namespace LanguageWorkbench
{
	public partial class App : Application
	{
		public App()
		{
			Startup += Application_Startup;
			Exit += Application_Exit;
			UnhandledException += Application_UnhandledException;

			InitializeComponent();
		}

		private void Application_Startup(object sender, StartupEventArgs e)
		{
			if (Current.IsRunningOutOfBrowser)
			{
				Current.MainWindow.WindowState = WindowState.Maximized;
			}

			if (Current.IsRunningOutOfBrowser && Current.HasElevatedPermissions)
			{
				CheckAndDownloadUpdate();
			}

			StyleManager.ApplicationTheme = new SummerTheme();

			var u = new WorkBenchView();
			var vm = new WorkBenchViewModel();
			u.ViewModel = vm;
			RootVisual = u;
		}


		public void CheckAndDownloadUpdate()
		{
			var app = Current;

			app.CheckAndDownloadUpdateCompleted += (sender, e) =>
			{
				if (e.UpdateAvailable)
				{
					MessageBox.Show("A new version of this application has been downloaded. Please close and start this application again for update to reflect.");

					RestartOutOfBrowser();
				}
				else if (e.Error != null)
				{
					MessageBox.Show(
						"An application update is available, but an error has occurred.\n" +
						"This can happen, for example, when the update requires\n" +
						"a new version of Silverlight or requires elevated trust.\n" +
						"To install the update, visit the application home page.");

					RestartOutOfBrowser();
				}
			};

			app.CheckAndDownloadUpdateAsync();
		}

		private void RestartOutOfBrowser()
		{
			var app = Current;
			if (app.IsRunningOutOfBrowser && app.HasElevatedPermissions)
			{
				app.MainWindow.Close();
			}
		}

		private void Application_Exit(object sender, EventArgs e)
		{
		}

		private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
		{
			// If the app is running outside of the debugger then report the exception using
			// the browser's exception mechanism. On IE this will display it a yellow alert 
			// icon in the status bar and Firefox will display a script error.
			if (!Debugger.IsAttached)
			{
				// NOTE: This will allow the application to continue running after an exception has been thrown
				// but not handled. 
				// For production applications this error handling should be replaced with something that will 
				// report the error to the website and stop the application.
				e.Handled = true;
				Deployment.Current.Dispatcher.BeginInvoke(delegate { ReportErrorToDOM(e); });
			}
		}

		private void ReportErrorToDOM(ApplicationUnhandledExceptionEventArgs e)
		{
			try
			{
				string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
				errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

				HtmlPage.Window.Eval("throw new Error(\"Unhandled Error in Silverlight Application " + errorMsg + "\");");
			}
			catch (Exception)
			{
			}
		}
	}
}