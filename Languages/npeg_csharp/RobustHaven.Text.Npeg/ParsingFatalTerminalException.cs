using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RobustHaven.Text.Npeg
{
    public class ParsingFatalTerminalException : ParsingExpressionGrammarException
    {
        public ParsingFatalTerminalException(String message)
            : base(message)
        { }
    }
}
