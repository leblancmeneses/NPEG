using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RobustHaven.Text.Npeg
{
    public class IteratorUsageException : ParsingExpressionGrammarException
    {
        public IteratorUsageException(string message)
            : base(message)
        {
        }
    }
}
