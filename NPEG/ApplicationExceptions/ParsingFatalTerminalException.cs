using System;

namespace NPEG.ApplicationExceptions
{
  public class ParsingFatalTerminalException : NpegException
  {
    public ParsingFatalTerminalException()
        : base("Parse tree encountered an invalid path of execution.")
    {
    }

    public ParsingFatalTerminalException(String message)
        : base(message)
    {
    }

    public ParsingFatalTerminalException(String message, Exception innerException)
        : base(message, innerException)
    {
    }
  }
}
