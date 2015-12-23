using System.ServiceProcess;

namespace SoftDojo.Shihan.WinSvc
{
	class Program
	{
		static void Main()
		{
			ServiceBase.Run(new ServiceBase[] { new ShihanWinService() });
		}
	}
}
