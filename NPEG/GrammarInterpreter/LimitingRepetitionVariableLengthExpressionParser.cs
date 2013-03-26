using NPEG.ApplicationExceptions;

namespace NPEG.GrammarInterpreter
{
	public class LimitingRepetitionVariableLengthExpressionParser
	{
		public AstNode Parse(IInputIterator inputIterator)
		{
			string grammar = @"
				WhiteSpace: [\s\n\t ]+;
				Comment: '/*' (!'*/' .)* '*/';
				(?<S>): (WhiteSpace / Comment)*;
				(?<Variable>): '\k<' S (?<Name>[a-zA-Z][a-zA-Z0-9]*) S '>';
				(?<Digit>): [0-9]+('.'[0-9]+)?;
				Value: Variable / Digit / '(' S Expr S ')';
				(?<Product \rsc>): Value S ((?<Symbol> '*' / '/') S Value)*;
				(?<Sum \rsc>): Product S ((?<Symbol>'+' / '-') S Product)*;
				(?<Expr \rsc>): S Sum S;
            ".Trim();

			AExpression rules = PEGrammar.Load(grammar);

			var visitor = new NpegParserVisitor(inputIterator);
			rules.Accept(visitor);

			if (visitor.IsMatch)
			{
				return visitor.AST;
			}

			throw new InvalidInputException();
		}
	}
}
