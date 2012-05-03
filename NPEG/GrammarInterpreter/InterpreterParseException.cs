using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NPEG.GrammarInterpreter
{
    public class InterpreterParseException : ApplicationExceptions.ModelException
    {
        public InterpreterParseException(String message):base(message)
        { 
        
        }
    }
}
