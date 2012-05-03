using System;

namespace NPEG.GrammarInterpreter
{
	public class StatementAstNode : IAstNodeReplacement
	{
		public String Name
		{
			get
			{
				if (IsCaptured)
				{
					return Children[0].Children[0].Children[0].Token.Value;
				}
				else
				{
					return Children[0].Token.Value;
				}
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
				else
				{
					// it's a label and will not be captured
					return false;
				}
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