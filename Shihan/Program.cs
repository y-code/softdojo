using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;

namespace SoftDojo.Shihan
{
	public class Program
	{
		private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private static Program _serverContext;

		public static Program ServerContext
		{
			get
			{
				if (_serverContext == null)
					_serverContext = new Program();
				return _serverContext;
			}
		}

		static void Main(string[] args)
		{
			System.Console.CancelKeyPress += new ConsoleCancelEventHandler((sender, eventArgs) => Program.ServerContext.Stop());

			Program.ServerContext.Start();

			while (Program.ServerContext.Status != ProgramStatus.Stopped)
				Thread.Sleep(500);
		}

		public ServiceHost ServiceHost { get; private set; }

		public ProgramStatus Status { get; private set; }

		public Dictionary<string, Client> Clients { get; private set; }

		private Timer _serverHeart;

		private Program()
		{
			Status = ProgramStatus.Stopped;
			Clients = new Dictionary<string, Client>();
			_serverHeart = new Timer((state) => Process(), this, new TimeSpan(0, 0, 1), new TimeSpan(0, 0, 1));
		}

		public void Start()
		{
			Status = ProgramStatus.Starting;
			_log.Info("WCF Service is starting...");

			if (ServiceHost != null)
			{
				ServiceHost.Close();
			}

			ServiceHost = new ServiceHost(typeof(WCFService));
			ServiceHost.Open();

			Status = ProgramStatus.Running;
			_log.Info("WCF Service is running...");
		}

		public void Stop()
		{
			Status = ProgramStatus.Stopping;
			_log.Info("WCF Service is stopping...");

			ServiceHost.Close();
			ServiceHost = null;

			Status = ProgramStatus.Stopped;
			_log.Info("WCF Service stopped...");
		}

		private void Process()
		{
			foreach (var client in Clients.Values)
			{
				lock (client)
				{
					switch(client.State)
					{
						case Client.ClientState.Closing:
							_log.Info("A client is closing");
							client.Close();
							_log.Info("A client has closed");
							break;
						case Client.ClientState.Idling:
							if (client.CallbackChannel.State == CommunicationState.Opened)
							{
								var work = "Work A";
								client.StartWork(work);
							}
							else
							{
								_log.Info("A client has been lost.");
								client.Close();
							}
							break;
					}
				}
			}
		}
	}

	public enum ProgramStatus
	{
		Starting,
		Running,
		Stopping,
		Stopped
	}
}
