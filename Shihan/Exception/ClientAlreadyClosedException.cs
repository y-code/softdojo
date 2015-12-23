
namespace SoftDojo.Shihan.Exception
{
	public class ClientAlreadyClosedException : System.Exception
	{
		public ClientAlreadyClosedException()
			: base()
		{
		}

		public ClientAlreadyClosedException(string message)
			: base(message)
		{
		}
	}
}
