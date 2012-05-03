using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NPEG.ApplicationExceptions
{
    public class InvalidRuleException : ModelException
    {
        public InvalidRuleException()
            : base("Supplied grammar rules could not be compiled into a parse tree.")
        { }

        public InvalidRuleException(String message)
            : base(message)
        { }

        public InvalidRuleException(String message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
