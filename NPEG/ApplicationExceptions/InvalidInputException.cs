using System;

namespace NPEG.ApplicationExceptions
{
  public class InvalidInputException : NpegException
  {
    public InvalidInputException()
        : base("Supplied input could not be parsed by compiled parse tree.")
    {
    }

    public InvalidInputException(String message)
        : base(message)
    {
    }

    public InvalidInputException(String message, Exception innerException)
        : base(message, innerException)
    {
    }
  }
}
