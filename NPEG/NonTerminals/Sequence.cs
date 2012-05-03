using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NPEG.NonTerminals
{
	[DataContract]
	public class Sequence : AComposite
	{
		private readonly AExpression left;
		private readonly AExpression right;

		public Sequence(AExpression left, AExpression right)
		{
			this.left = left;
			this.right = right;
		}

		[DataMember]
		public override List<AExpression> Children
		{
			get { return new List<AExpression> {left, right}; }
		}
	}
}