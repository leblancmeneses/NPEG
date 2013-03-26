using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace NPEG
{
	[DataContract]
	[DebuggerDisplay("AstNode: {(Token == null)? string.Empty:Token.Name}, Children {Children.Count}")]
	public class AstNode
	{
		public AstNode()
		{
			Children = new List<AstNode>();
		}

		// removed datamember else wcf service recursively tries to serialize.
		// eventually crashing the service.
		public AstNode Parent { get; set; }

		[DataMember]
		public List<AstNode> Children { get; set; }

		[DataMember]
		public TokenMatch Token { get; set; }


		public void Accept(IAstNodeReplacement visitor)
		{
			visitor.VisitEnter(this);

			foreach (AstNode node in Children)
			{
				node.Accept(visitor);
			}

			visitor.VisitLeave(this);
		}
	}
}