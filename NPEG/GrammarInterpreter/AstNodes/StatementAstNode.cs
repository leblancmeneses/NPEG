using System;
using NPEG.Extensions;

namespace NPEG.GrammarInterpreter.AstNodes
{
	public class StatementAstNode : IAstNodeReplacement
	{
		private readonly IInputIterator _inputIterator;

		public StatementAstNode(IInputIterator inputIterator)
		{
			_inputIterator = inputIterator;
		}

		public String Name
		{
			get
			{
				if (IsCaptured)
				{
					return Children[0].Children[0].Children[0].Token.ValueAsString(_inputIterator);
				}

				return Children[0].Token.ValueAsString(_inputIterator);
			}
		}

		public Boolean IsCaptured
		{
			get
			{
				if (Children[0].Children[0].Token.Name == "TerminalReference")
				{
					return true;
				}

				// it's a label and will not be captured
				return false;
			}
		}

		public override void VisitEnter(AstNode node)
		{
		}

		public override void VisitLeave(AstNode node)
		{
		}
	}
}