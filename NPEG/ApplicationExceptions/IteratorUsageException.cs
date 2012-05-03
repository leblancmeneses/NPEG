using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NPEG.ApplicationExceptions
{
    public class IteratorUsageException : ModelException
    {
        public IteratorUsageException(string message)
            : base(message)
        {
        }
    }
}
