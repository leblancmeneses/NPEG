using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NPEG.NonTerminals
{
  [DataContract]
  public class Optional : AComposite
  {
    private readonly AExpression Exp;

    public Optional(AExpression exp)
    {
      Exp = exp;
    }

    [DataMember]
    public override List<AExpression> Children
    {
      get { return new List<AExpression> { Exp }; }
    }
  }
}
