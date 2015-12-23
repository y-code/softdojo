using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

namespace SoftDojo.Shihandai
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private ConsoleOutRedirectWriter consoleOutRedirect = new ConsoleOutRedirectWriter();
		private ConsoleErrRedirectWriter consoleErrRedirect = new ConsoleErrRedirectWriter();

		private Program _client;

		public MainWindow()
		{
			InitializeComponent();

			CmdRegister.IsEnabled = true;
			CmdUnregister.IsEnabled = false;
		}

		private void CmdRegister_Click(object sender, RoutedEventArgs e)
		{
			CmdRegister.IsEnabled = false;

			var bkWorker = new BackgroundWorker();
			bkWorker.DoWork += delegate(object s, DoWorkEventArgs args)
			{
				_client = new Program();
				_client.Join();

				Dispatcher.BeginInvoke((Action)delegate() { CmdUnregister.IsEnabled = true; });
			};
			bkWorker.RunWorkerAsync();
		}

		private void CmdUnregister_Click(object sender, RoutedEventArgs e)
		{
			CmdUnregister.IsEnabled = false;

			var bkWorker = new BackgroundWorker();
			bkWorker.DoWork += delegate(object s, DoWorkEventArgs eargs)
			{
				_client.Leave();

				Dispatcher.BeginInvoke((Action)delegate() { CmdRegister.IsEnabled = true; });
			};
			bkWorker.RunWorkerAsync();
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
	}
}
