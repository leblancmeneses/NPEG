using System;
using System.Collections.Generic;

namespace RobustHaven.Text.Npeg
{
	public class AstNode
	{
		public AstNode()
		{
			Children = new List<AstNode>();
		}

		public AstNode Parent { get; set; }

		public List<AstNode> Children { get; set; }

		public TokenMatch Token { get; set; }


		public void Accept(IAstNodeVisitor visitor)
		{
			visitor.VisitEnter(this);

			Boolean isFirstTime = true;
			foreach (AstNode node in Children)
			{
				if (!isFirstTime)
				{
					visitor.VisitExecute(this);
				}

				isFirstTime = false;
				node.Accept(visitor);
			}

			visitor.VisitLeave(this);
		}
	}
}