using System;
using NPEG.ApplicationExceptions;

namespace NPEG.GrammarInterpreter
{
	public class InterpreterParseException : ModelException
	{
		public InterpreterParseException(String message) : base(message)
		{
		}
	}
}