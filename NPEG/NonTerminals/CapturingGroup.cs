using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NPEG.NonTerminals
{
  [DataContract]
  public class CapturingGroup : AComposite
  {
    private readonly AExpression Exp;

    public CapturingGroup(String uniquename, AExpression exp)
    {
      Exp = exp;
      Name = uniquename;
      DoReplaceBySingleChildNode = false;
      DoCreateCustomAstNode = false;
    }

    [DataMember]
    public String Name { get; set; }

    [DataMember]
    public Boolean DoReplaceBySingleChildNode { get; set; }

    [DataMember]
    public Boolean DoCreateCustomAstNode { get; set; }


    [DataMember]
    public override List<AExpression> Children
    {
      get { return new List<AExpression> { Exp }; }
    }
  }
}
