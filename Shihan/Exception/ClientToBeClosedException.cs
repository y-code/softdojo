
namespace SoftDojo.Shihan.Exception
{
	public class ClientToBeClosedException : System.Exception
	{
		public ClientToBeClosedException()
			: base()
		{
		}

		public ClientToBeClosedException(string message)
			: base(message)
		{
		}
	}
}
