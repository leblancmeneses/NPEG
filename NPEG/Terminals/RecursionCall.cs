using System;
using System.Runtime.Serialization;

namespace NPEG.Terminals
{
  [DataContract]
  public class RecursionCall : ALeaf
  {
    public RecursionCall(String bindunique)
    {
      FunctionName = bindunique;
    }

    [DataMember]
    public String FunctionName { get; set; }
  }
}
