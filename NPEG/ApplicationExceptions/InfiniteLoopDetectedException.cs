using System;

namespace NPEG.ApplicationExceptions
{
	public class InfiniteLoopDetectedException : ModelException
	{
		public InfiniteLoopDetectedException()
			: base("Supplied grammar rules caused an infinite loop.")
		{
		}

		public InfiniteLoopDetectedException(String message)
			: base(message)
		{
		}

		public InfiniteLoopDetectedException(String message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}