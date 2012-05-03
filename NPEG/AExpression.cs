using System.Runtime.Serialization;

namespace NPEG
{
	[DataContract]
	public abstract class AExpression
	{
		public abstract void Accept(IParseTreeVisitor visitor);
	}
}