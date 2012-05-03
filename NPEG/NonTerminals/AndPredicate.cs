using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NPEG.NonTerminals
{
	[DataContract]
	public class AndPredicate : AComposite
	{
		private readonly AExpression exp;

		public AndPredicate(AExpression exp)
		{
			this.exp = exp;
		}

		[DataMember]
		public override List<AExpression> Children
		{
			get { return new List<AExpression> {exp}; }
		}
	}
}