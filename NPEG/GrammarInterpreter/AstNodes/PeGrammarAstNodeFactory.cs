using System;

namespace NPEG.GrammarInterpreter.AstNodes
{
	internal class PeGrammarAstNodeFactory : IAstNodeFactory
	{
		private readonly IInputIterator _inputIterator;

		public PeGrammarAstNodeFactory(IInputIterator inputIterator)
		{
			_inputIterator = inputIterator;
		}

		#region IAstNodeFactory Members

		public IAstNodeReplacement Create(AstNode original)
		{
			switch (original.Token.Name)
			{
				case "Statement":
					return new StatementAstNode(_inputIterator);
				case "PEG":
					return new InterpreterAstNode(_inputIterator);
			}

			throw new ArgumentOutOfRangeException(
				String.Format("PeGrammarAstNodeFactory does not define replacement node for: {0}", original.Token.Name));
		}

		#endregion
	}
}