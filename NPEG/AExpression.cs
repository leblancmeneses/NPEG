using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace NPEG
{
    [DataContract]
    abstract public class AExpression
    {
        abstract public void Accept(IParseTreeVisitor visitor);
    }
}
