using System;
using NPEG.GrammarInterpreter;

namespace NPEG.Service
{
	public class NpegParsingService : IParsingService
	{
		#region IParsingService Members

		public NpegData Parse(String rules, Byte[] input)
		{
			var result = new NpegData {Ast = null, ParseTree = null};

			AExpression parseTree = PEGrammar.Load(rules);
			var visitor = new NpegParserVisitor(
				new StringInputIterator(input)
				);
			parseTree.Accept(visitor);

			if (!visitor.IsMatch)
			{
				result.Warnings = visitor.Warnings;
			}

			result.Warnings = visitor.Warnings;
			result.Ast = visitor.AST;
			result.ParseTree = parseTree;

			return result;
		}

		#endregion
	}
}