using System;

namespace NPEG.ApplicationExceptions
{
	public class ModelException : ApplicationException
	{
		public ModelException()
		{
		}

		public ModelException(String message)
			: base(message)
		{
		}

		public ModelException(String message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}