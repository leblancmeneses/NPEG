using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NPEG
{
    public interface IAstNodeFactory
    {
        IAstNodeReplacement Create(AstNode original);
    }
}
