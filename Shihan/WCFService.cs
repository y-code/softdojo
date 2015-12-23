using log4net;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;

namespace SoftDojo.Shihan
{
	[ServiceContract(
		CallbackContract = typeof(IShihandaiServiceToShihan),
		SessionMode = SessionMode.Required
		)]
	public interface IShihanServiceToShihandai
	{
		[OperationContract]
		string Enroll();

		[OperationContract]
		string Detach();

		[OperationContract]
		string FinalizeWork();
	}

	[ServiceContract]
	public interface IShihandaiServiceToShihan
	{
		[OperationContract]
		string StartWork(string work);
	}

	[ServiceBehavior]
	public class WCFService : IShihanServiceToShihandai
	{
		private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public string Enroll()
		{
			var client = new Client(OperationContext.Current);

			if (Program.ServerContext.Clients.ContainsKey(client.Address))
				Program.ServerContext.Clients.Remove(client.Address);
			Program.ServerContext.Clients.Add(client.Address, client);

			return "Welcome!";
		}

		public string Detach()
		{
			var clientProp = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;

			if (Program.ServerContext.Clients.ContainsKey(clientProp.Address))
				Program.ServerContext.Clients[clientProp.Address].BeginClosing();

			return "You've done a lot. Thanks!";
		}

		public string FinalizeWork()
		{
			var clientProp = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;

			if (Program.ServerContext.Clients.ContainsKey(clientProp.Address))
			{
				Program.ServerContext.Clients[clientProp.Address].EndWork();
			}

			return "Good job!";
		}
	}
}
