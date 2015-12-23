using log4net;
using SoftDojo.Shihandai.ShihanService;
using System;
using System.ComponentModel;
using System.Reflection;
using System.ServiceModel;
using System.Threading;

namespace SoftDojo.Shihandai
{
	class Program : IShihanServiceToShihandaiCallback
	{
		private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public ShihanServiceToShihandaiClient ShihanService { get; private set; }

		public InstanceContext Context { get; private set; }

		public Program()
		{
			Context = new InstanceContext(this);
		}

		public void Join()
		{
			_log.Info("Joining to the server...");

			ShihanService = new ShihanServiceToShihandaiClient(Context);
			var greeting = ShihanService.Enroll();

			System.Console.WriteLine("Return from SampleWCFService1.Enroll: " + greeting);

			_log.Info("This client has joined to the server.");
		}

		public void Leave()
		{
			_log.Info("Leaving from the server...");

			ShihanService.Detach();

			_log.Info("This client is going to leave the server.");
		}

		string IShihanServiceToShihandaiCallback.StartWork(string work)
		{
			var bkWorker = new BackgroundWorker();
			bkWorker.DoWork += delegate(object sender, DoWorkEventArgs eventArgs)
			{
				_log.Info("Starting " + work + "...");

				Thread.Sleep(1000);

				ShihanService.FinalizeWork();

				_log.Info("Finished " + work + "...");
			};
			bkWorker.RunWorkerAsync();

			return "started";
		}
	}
}
