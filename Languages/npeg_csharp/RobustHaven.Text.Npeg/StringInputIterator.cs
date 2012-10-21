using System;
using System.Diagnostics;
using System.Text;

namespace RobustHaven.Text.Npeg
{
	public class StringInputIterator : InputIterator
	{
		private readonly Byte[] input;

		public StringInputIterator(Byte[] input)
			: base(input.Length)
		{
			this.input = input;
		}

		public StringInputIterator(String us_ascii)
			: base(Encoding.ASCII.GetBytes(us_ascii).Length)
		{
			input = Encoding.ASCII.GetBytes(us_ascii);
			Index = 0;
		}


		public override byte[] Text(int start, int end)
		{
			var bytes = new byte[end - start];
			Array.Copy(input, start, bytes, 0, end - start);
			return bytes;
		}


		public override short Current()
		{
			if (Index >= Length) return -1;
			else
			{
				return input[Index];
			}
		}

		public override short Next()
		{
			if (Index >= Length) return -1;
			else
			{
				Index += 1;

				if (Index >= Length) return -1;
				else return input[Index];
			}
		}

		public override short Previous()
		{
			if (Index <= 0)
			{
				return -1;
			}
			else
			{
				Debug.Assert(Length > 0);

				Index -= 1;

				return input[Index];
			}
		}
	}
}