using System.ServiceProcess;

namespace SoftDojo.Shihan.WinSvc
{
	public partial class ShihanWinService : ServiceBase
	{
		public const string SERVICE_NAME = "Sample WCF Service";

		public ShihanWinService()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			if (SoftDojo.Shihan.Program.ServerContext.Status != Shihan.ProgramStatus.Stopping
				&& SoftDojo.Shihan.Program.ServerContext.Status != Shihan.ProgramStatus.Stopped)
				SoftDojo.Shihan.Program.ServerContext.Stop();

			SoftDojo.Shihan.Program.ServerContext.Start();
		}

		protected override void OnStop()
		{
			if (SoftDojo.Shihan.Program.ServerContext.Status != Shihan.ProgramStatus.Stopping
				&& SoftDojo.Shihan.Program.ServerContext.Status != Shihan.ProgramStatus.Stopped)
				SoftDojo.Shihan.Program.ServerContext.Stop();
		}
	}
}
