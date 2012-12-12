using System;

namespace NPEG
{
	public abstract class IInputIterator
	{
		protected IInputIterator(Int32 length)
		{
			Index = 0;
			Length = length;
		}

		// Zero based index
		public Int32 Index { get; set; }

		public Int32 Length { get; private set; }


		public abstract Byte[] Text(Int32 start, Int32 end);


		// Instead of returning a Byte
		// an Int16 is used so error condition is encoded with value.
		// if the sign bit is flagged we know an error occurred.
		// 100000000 00000000 == -1
		// if sign bit is not enabled 
		// we know casting (Byte)int16value will provide valid data of stream.
		// Boolean MoveNext()
		// Boolean MoveBack()
		// Boolean IsCurrentValid() used inconjuction with Byte Current()

		public abstract Int16 Current();
		public abstract Int16 Next();
		public abstract Int16 Previous();
	}
}