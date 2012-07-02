using System;
using NPEG.ApplicationExceptions;

namespace NPEG.GrammarInterpreter
{
	public class InterpreterParseException : NpegException
	{
		public InterpreterParseException(String message) : base(message)
		{
		}
	}
}