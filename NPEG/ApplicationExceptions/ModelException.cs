using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NPEG.ApplicationExceptions
{
    public class ModelException : System.ApplicationException
    {
        public ModelException() : base() { }

        public ModelException(String message)
            : base(message)
        { }

        public ModelException(String message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
