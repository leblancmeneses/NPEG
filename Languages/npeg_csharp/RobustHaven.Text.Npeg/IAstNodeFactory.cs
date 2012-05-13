using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RobustHaven.Text.Npeg
{
    public interface IAstNodeFactory
    {
        IAstNodeReplacement Create(AstNode original);
    }
}
