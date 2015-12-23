using System.ComponentModel;
using System.ServiceProcess;

namespace SoftDojo.Shihan.WinSvc
{
	[RunInstaller(true)]
	public class Installer : System.Configuration.Install.Installer
	{
		private ServiceProcessInstaller process;
		private ServiceInstaller service;

		public Installer()
		{
			process = new ServiceProcessInstaller();
			process.Account = ServiceAccount.LocalSystem;
			service = new ServiceInstaller();
			service.ServiceName = ShihanWinService.SERVICE_NAME;
			Installers.Add(process);
			Installers.Add(service);
		}
	}
}
