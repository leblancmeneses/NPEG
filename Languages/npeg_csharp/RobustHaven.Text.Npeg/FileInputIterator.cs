using System;
using System.Diagnostics;
using System.IO;

namespace RobustHaven.Text.Npeg
{
	public class FileInputIterator : InputIterator
	{
		private readonly Stream input;

		public FileInputIterator(Stream input)
			: base((Int32) input.Length)
		{
			this.input = input;
		}

		public override byte[] Text(int start, int end)
		{
			var buffer = new byte[end - start];
			input.Read(buffer, start, end - start);
			return buffer;
		}

		public override short Current()
		{
			if (Index >= Length) return -1;
			else
			{
				return GetByte(Index);
			}
		}

		public override short Next()
		{
			if (Index >= Length) return -1;
			else
			{
				Index += 1;

				if (Index >= Length) return -1;
				else return GetByte(Index);
				;
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

				return GetByte(Index);
				;
			}
		}


		private short GetByte(Int32 index)
		{
			var b = new Byte[1];
			input.Read(b, Index, 1);
			return b[0];
		}
	}
}