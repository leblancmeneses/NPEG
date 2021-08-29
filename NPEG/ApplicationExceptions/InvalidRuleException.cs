using System;

namespace NPEG.ApplicationExceptions
{
  public class InvalidRuleException : NpegException
  {
    public InvalidRuleException()
        : base("Supplied grammar rules could not be compiled into a parse tree.")
    {
    }

    public InvalidRuleException(String message)
        : base(message)
    {
    }

    public InvalidRuleException(String message, Exception innerException)
        : base(message, innerException)
    {
    }
  }
}
