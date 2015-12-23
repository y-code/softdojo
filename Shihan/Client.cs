using log4net;
using System;
using System.ComponentModel;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace SoftDojo.Shihan
{
	public class Client
	{
		private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public enum ClientState
		{
			Idling,
			Working,
			Closing,
			Closed
		}
		public string Address { get; private set; }
		public IChannel CallbackChannel { get; private set; }
		public IShihandaiServiceToShihan SampleWCFClient1 { get; private set; }
		internal ClientState State { get; private set; }
		public bool IsClosing = false;
		public string Work { get; private set; }

		public Client(OperationContext operationContext)
		{
			var clientProp = operationContext.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
			Address = clientProp.Address;
			SampleWCFClient1 = operationContext.GetCallbackChannel<IShihandaiServiceToShihan>();
			CallbackChannel = SampleWCFClient1 as IChannel;
			State = ClientState.Idling;
		}

		public void BeginClosing()
		{
			lock (this)
			{
				switch (State)
				{
					case ClientState.Idling:
						State = ClientState.Closing;
						break;
					case ClientState.Working:
						IsClosing = true;
						break;
					case ClientState.Closing:
						break;
					case ClientState.Closed:
						break;
				}
			}
		}

		public void Close()
		{
			lock (this)
			{
				switch (State)
				{
					case ClientState.Working:
						throw new Exception.ClosingClientAtWorkException();
					case ClientState.Idling:
					case ClientState.Closing:
						State = ClientState.Closed;
						if (CallbackChannel.State == CommunicationState.Opened)
							CallbackChannel.Close();
						break;
					case ClientState.Closed:
						break;
				}
			}
		}

		public void StartWork(string work)
		{
			switch (State)
			{
				case ClientState.Idling:
					if (CallbackChannel.State == CommunicationState.Opened)
					{
						State = ClientState.Working;
						Work = work;
						var bgWorker = new BackgroundWorker();
						bgWorker.DoWork += delegate(object sender, DoWorkEventArgs eventArgs)
						{
							var result = SampleWCFClient1.StartWork(work);
							_log.Info("[" + Address + "] " + work + " : " + result);
						};
						bgWorker.RunWorkerAsync();
					}
					else
					{
						State = ClientState.Closed;
					}
					break;
				case ClientState.Working:
					throw new Exception.RequestingAnotherWorkToClientAtWorkException();
				case ClientState.Closing:
					throw new Exception.ClientToBeClosedException();
				case ClientState.Closed:
					throw new Exception.ClientAlreadyClosedException();
			}
		}

		public void EndWork()
		{
			lock (this)
			{
				switch (State)
				{
					case ClientState.Idling:
						throw new Exception.NoWorkToEndException();
					case ClientState.Working:
						_log.Info("[" + Address + "] " + Work + " : ended");
						if (IsClosing)
							State = ClientState.Closing;
						else
							State = ClientState.Idling;
						break;
					case ClientState.Closing:
						throw new Exception.ClientToBeClosedException();
					case ClientState.Closed:
						throw new Exception.ClientAlreadyClosedException();
				}
			}
		}
	}
}
