using System;
using System.Runtime.Serialization;

namespace NPEG.Terminals
{
  // #32 (decimal utf-8 codepoint) // Match input against the specified unicode character (can consume 1-4 bytes of input)
  // #x3A0 (hex)
  // #b111 (binary)
  [DataContract]
  public class CodePoint : ALeaf
  {
    // for hex and binary you can group and input iterator will be matched against binary array.
    // so instead of #x61 #x62 #63 to match abc  you could write a short hand: #616263 

    // data is shifted right when input form incomplete byte boundaries
    // Example:  0xFAB
    // to complete byte boundaris
    // does the user mean: FAB0 or 0FAB ?
    // This system shifts all data to the right to complete byte boundaries.
    // so 0xFAB would become 0FAB and 2 bytes of input iterator would be consumed.


    [DataMember]
    public String Match { get; set; }
  }
}
