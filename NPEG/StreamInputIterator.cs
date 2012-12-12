using System;
using System.Diagnostics;
using System.IO;
using NPEG.ApplicationExceptions;

namespace NPEG
{
	public class StreamInputIterator : IInputIterator
	{
		private readonly Stream _input;

		public StreamInputIterator(Stream input)
			: base((Int32) input.Length)
		{
			_input = input;
		}

		public override byte[] Text(int start, int end)
		{
			if (start > end)
				throw new IteratorUsageException("Index out of range. End must be >= than Start.  byte[] Text(int start, int end)");

			var buffer = new byte[end - start + 1];
			_input.Position = start;
			_input.Read(buffer, 0, end - start + 1);
			return buffer;
		}

		public override short Current()
		{
			if (Index >= Length) return -1;
			return GetByte(Index);
		}

		public override short Next()
		{
			if (Index >= Length) return -1;
			Index += 1;

			if (Index >= Length) return -1;
			return GetByte(Index);
		}

		public override short Previous()
		{
			if (Index <= 0)
			{
				return -1;
			}
			Debug.Assert(Length > 0);

			Index -= 1;

			return GetByte(Index);
		}


		private short GetByte(Int32 index)
		{
			var b = new Byte[1];
			_input.Position = index;
			_input.Read(b, 0, 1);
			return b[0];
		}
	}
}