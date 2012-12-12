using System;
using System.Diagnostics;
using NPEG.ApplicationExceptions;

namespace NPEG
{
	public class ByteInputIterator : IInputIterator
	{
		private readonly Byte[] _input;

		public ByteInputIterator(Byte[] input)
			: base(input.Length)
		{
			_input = input;
		}
		
		public override byte[] Text(int start, int end)
			//start and end are zero indexed.
		{
			Int32 destinationSize = 0;
			if (end >= start)
			{
				destinationSize = end - start + 1;
			}
			else
			{
				throw new IteratorUsageException("Index out of range. End must be >= than Start.  byte[] Text(int start, int end)");
			}

			if (Length == 0)
				return new byte[0];

			var destination = new byte[destinationSize];
			Array.Copy(_input, start, destination, 0, destinationSize);
			return destination;
		}


		public override short Current()
		{
			if (Index >= Length) return -1;
			return _input[Index];
		}

		public override short Next()
		{
			if (Index >= Length) return -1;
			Index += 1;

			if (Index >= Length) return -1;
			return _input[Index];
		}

		public override short Previous()
		{
			if (Index <= 0)
			{
				return -1;
			}
			Debug.Assert(Length > 0);

			Index -= 1;

			return _input[Index];
		}
	}
}