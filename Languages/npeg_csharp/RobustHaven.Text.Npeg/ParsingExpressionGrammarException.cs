using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RobustHaven.Text.Npeg
{
    public class ParsingExpressionGrammarException : ApplicationException
    {
        public ParsingExpressionGrammarException(String message)
            : base(message)
        { }
    }
}
