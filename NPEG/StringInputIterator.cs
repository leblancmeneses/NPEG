using System;
using System.Diagnostics;
using System.Text;
using NPEG.ApplicationExceptions;

namespace NPEG
{
	public class StringInputIterator : IInputIterator
	{
		private readonly Byte[] input;

		public StringInputIterator(Byte[] input)
			: base(input.Length)
		{
			this.input = input;
			Index = 0;
		}

		public StringInputIterator(String utf8)
			: base(Encoding.UTF8.GetBytes(utf8).Length)
		{
			input = Encoding.UTF8.GetBytes(utf8);
			Index = 0;
		}

		public override byte[] Text(int start, int end)
			//start and end are zero indexed.
		{
			Int32 length = 0;
			if (end >= start)
			{
				length = end - start + 1; // size is not zero indexed so +1
			}
			else
			{
				throw new IteratorUsageException("Index out of range. End must be >= than Start.  byte[] Text(int start, int end)");
			}
			var destination = new byte[length];
			Array.Copy(input, start, destination, 0, length);
			return destination;
		}


		public override short Current()
		{
			if (Index >= Length) return -1;
			return input[Index];
		}

		public override short Next()
		{
			if (Index >= Length) return -1;
			Index += 1;

			if (Index >= Length) return -1;
			return input[Index];
		}

		public override short Previous()
		{
			if (Index <= 0)
			{
				return -1;
			}
			Debug.Assert(Length > 0);

			Index -= 1;

			return input[Index];
		}
	}
}