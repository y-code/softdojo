using System;
using System.Collections.Generic;
using System.IO;

namespace SoftDojo.Shihan.WinApp
{
	public class RedirectWriter : StringWriter
	{

		public Action<String> OnWrite;

		private void WriteGeneric<T>(T value) { if (OnWrite != null) OnWrite(value.ToString()); }


		public override void Write(char value) { WriteGeneric<char>(value); }
		public override void Write(string value) { WriteGeneric<string>(value); }
		public override void Write(bool value) { WriteGeneric<bool>(value); }
		public override void Write(int value) { WriteGeneric<int>(value); }
		public override void Write(double value) { WriteGeneric<double>(value); }
		public override void Write(long value) { WriteGeneric<long>(value); }

		private void WriteLineGeneric<T>(T value) { if (OnWrite != null) OnWrite(value.ToString() + "\n"); }
		public override void WriteLine(char value) { WriteLineGeneric<char>(value); }
		public override void WriteLine(string value) { WriteLineGeneric<string>(value); }
		public override void WriteLine(bool value) { WriteLineGeneric<bool>(value); }
		public override void WriteLine(int value) { WriteLineGeneric<int>(value); }
		public override void WriteLine(double value) { WriteLineGeneric<double>(value); }
		public override void WriteLine(long value) { WriteLineGeneric<long>(value); }

		public override void Write(char[] buffer, int index, int count)
		{
			base.Write(buffer, index, count);
			char[] buffer2 = new char[count]; //Ensures large buffers are not a problem
			for (int i = 0; i < count; i++) buffer2[i] = buffer[index + i];
			WriteGeneric<char[]>(buffer2);
		}

		public override void WriteLine(char[] buffer, int index, int count)
		{
			base.Write(buffer, index, count);
			char[] buffer2 = new char[count]; //Ensures large buffers are not a problem
			for (int i = 0; i < count; i++) buffer2[i] = buffer[index + i];
			WriteLineGeneric<char[]>(buffer2);
		}
	}

	public class ConsoleOutRedirectWriter : RedirectWriter
	{
		TextWriter consoleOut;

		public ConsoleOutRedirectWriter()
		{
			consoleOut = System.Console.Out;
			this.OnWrite += (text) =>
			{
				consoleOut.Write(text);
			};
			System.Console.SetOut(this);
		}

		public void Release()
		{
			System.Console.SetOut(consoleOut);
		}
	}

	public class ConsoleErrRedirectWriter : RedirectWriter
	{
		TextWriter consoleErr;

		public ConsoleErrRedirectWriter()
		{
			consoleErr = System.Console.Error;
			this.OnWrite += (text) =>
			{
				consoleErr.Write(text);
			};
			System.Console.SetError(this);
		}

		public void Release()
		{
			System.Console.SetError(consoleErr);
		}
	}
}
