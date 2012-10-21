using System;

namespace RobustHaven.Text.Npeg
{
	public class ParsingFatalTerminalException : ParsingExpressionGrammarException
	{
		public ParsingFatalTerminalException(String message)
			: base(message)
		{
		}
	}
}