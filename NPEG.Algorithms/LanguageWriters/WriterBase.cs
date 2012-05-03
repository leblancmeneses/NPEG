using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NPEG.Algorithms.LanguageWriters
{
    abstract public class WriterBase : NPEG.IParseTreeVisitor
    {
        abstract public void Write();
    }
}
