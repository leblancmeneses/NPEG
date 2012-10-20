using System;

namespace NPEG.ApplicationExceptions
{
	public class NpegException : Exception
	{
		public NpegException()
		{
		}

		public NpegException(String message)
			: base(message)
		{
		}

		public NpegException(String message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}