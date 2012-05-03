using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NPEG.ApplicationExceptions
{
    public class ParsingFatalTerminalException : ModelException
    {
        public ParsingFatalTerminalException()
            : base("Parse tree encountered an invalid path of execution.")
        { }

        public ParsingFatalTerminalException(String message)
            : base(message)
        { }

        public ParsingFatalTerminalException(String message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
