using System;
using System.Runtime.Serialization;

namespace NPEG
{
  [DataContract]
  public abstract class AExpression
  {
    public Int32 RuleStart { get; set; }

    public Int32 RuleEnd { set; get; }

    public abstract void Accept(IParseTreeVisitor visitor);
  }
}
