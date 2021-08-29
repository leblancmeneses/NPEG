using System;

namespace NPEG.GrammarInterpreter
{
  [Flags]
  public enum NpegOptions
  {
    Optimize = 0x2,
    Cached = 0x1,
    None = 0x0
  }
}
