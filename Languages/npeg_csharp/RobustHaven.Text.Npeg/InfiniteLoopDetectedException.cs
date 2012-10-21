using System;

namespace RobustHaven.Text.Npeg
{
	public class InfiniteLoopDetectedException : ParsingExpressionGrammarException
	{
		public InfiniteLoopDetectedException()
			: base("Supplied grammar rules caused an infinite loop.")
		{
		}

		public InfiniteLoopDetectedException(String message)
			: base(message)
		{
		}
	}
}