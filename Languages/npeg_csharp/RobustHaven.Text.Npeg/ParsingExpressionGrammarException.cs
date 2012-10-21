using System;

namespace RobustHaven.Text.Npeg
{
	public class ParsingExpressionGrammarException : ApplicationException
	{
		public ParsingExpressionGrammarException(String message)
			: base(message)
		{
		}
	}
}