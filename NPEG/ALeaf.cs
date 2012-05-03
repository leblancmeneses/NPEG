using System.Runtime.Serialization;

namespace NPEG
{
	[DataContract]
	public class ALeaf : AExpression
	{
		public override void Accept(IParseTreeVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}