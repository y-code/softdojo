using SoftDojo.Shihan;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SoftDojo.Shihan.WinApp
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private ConsoleOutRedirectWriter consoleOutRedirect = new ConsoleOutRedirectWriter();
		private ConsoleErrRedirectWriter consoleErrRedirect = new ConsoleErrRedirectWriter();

		public MainWindow()
		{
			InitializeComponent();

			CmdStart.IsEnabled = true;
			CmdStop.IsEnabled = false;
			CmdRestart.IsEnabled = false;

			Closing += CloseWindow;

			var writer = new StreamWriter(System.Console.OpenStandardOutput());

			var autoCmdStart = new MenuItemAutomationPeer(CmdStart);
			var invokeProvider = autoCmdStart.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
			invokeProvider.Invoke();
		}

		private void TxtConsole_Initialized(object sender, EventArgs e)
		{
			consoleOutRedirect.OnWrite += (output) => Dispatcher.BeginInvoke(DispatcherPriority.Normal,
				(Action<string>)delegate(string output2)
				{
					TxtConsole.AppendText(output2);
					TxtConsole.ScrollToEnd();
				}, output);
			consoleErrRedirect.OnWrite += (output) => Dispatcher.BeginInvoke(DispatcherPriority.Normal,
				(Action<string>)delegate(string output2)
				{
					TxtConsole.AppendText(output2);
					TxtConsole.ScrollToEnd();
				}, output);
		}

		private void CloseWindow(object sender, CancelEventArgs args)
		{
			CmdStop_Click(sender, null);

			consoleOutRedirect.Release();
			consoleErrRedirect.Release();
		}

		private void CmdStart_Click(object sender, RoutedEventArgs e)
		{
			if (Program.ServerContext.Status != ProgramStatus.Stopping
				&& Program.ServerContext.Status != ProgramStatus.Stopped)
			{
				Program.ServerContext.Stop();
			}

			Program.ServerContext.Start();

			CmdStart.IsEnabled = false;
			CmdStop.IsEnabled = true;
			CmdRestart.IsEnabled = true;
		}

		private void CmdStop_Click(object sender, RoutedEventArgs e)
		{
			if (Program.ServerContext.Status != ProgramStatus.Stopping
				&& Program.ServerContext.Status != ProgramStatus.Stopped)
			{
				Program.ServerContext.Stop();
			}

			CmdStart.IsEnabled = true;
			CmdStop.IsEnabled = false;
			CmdRestart.IsEnabled = false;
		}

		private void CmdRestart_Click(object sender, RoutedEventArgs e)
		{
			Program.ServerContext.Stop();
			Program.ServerContext.Start();

			CmdStart.IsEnabled = false;
			CmdStop.IsEnabled = true;
			CmdRestart.IsEnabled = true;
		}

	}
}
