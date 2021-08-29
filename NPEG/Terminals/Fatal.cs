using System;
using System.Runtime.Serialization;

namespace NPEG.Terminals
{
  [DataContract]
  public class Fatal : ALeaf
  {
    public Fatal()
    {
      Message = String.Empty;
    }

    [DataMember]
    public String Message { get; set; }
  }
}
