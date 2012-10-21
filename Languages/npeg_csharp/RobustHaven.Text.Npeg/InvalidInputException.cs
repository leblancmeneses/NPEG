namespace RobustHaven.Text.Npeg
{
	public class InvalidInputException : ParsingExpressionGrammarException
	{
		public InvalidInputException()
			: base("Supplied input could not be parsed by compiled parse tree.")
		{
		}
	}
}