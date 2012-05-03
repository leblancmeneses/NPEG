using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NPEG.NonTerminals
{
	[DataContract]
	public class ZeroOrMore : AComposite
	{
		private readonly AExpression Exp;

		public ZeroOrMore(AExpression exp)
		{
			Exp = exp;
		}

		[DataMember]
		public override List<AExpression> Children
		{
			get { return new List<AExpression> {Exp}; }
		}
	}
}