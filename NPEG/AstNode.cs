using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;

namespace NPEG
{
	[DataContract]
	[DebuggerDisplay("AstNode: {(Token == null)? string.Empty:Token.Name}, Children {Children.Count}")]
	public class AstNode
	{
		public AstNode()
		{
			Children = new AstNodeCollection();
		}

		// removed datamember else wcf service recursively tries to serialize.
		// eventually crashing the service.
		public AstNode Parent { get; set; }

		[DataMember]
		public AstNodeCollection Children { get; set; }

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

	public class AstNodeCollection : List<AstNode>
	{
		public AstNode this[string key]
		{
			get
			{
				return this.First(x=>x.Token.Name.Equals(key));
			}
			set
			{
				var node = this.First(x => x.Token.Name.Equals(key));
				node.Token = value.Token;
				node.Children = value.Children;
			}
		}
	}
}