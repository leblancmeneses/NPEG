using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NPEG
{
    // visitor
    abstract public class IAstNodeReplacement : AstNode
    {
        abstract public void VisitEnter(AstNode node);
        abstract public void VisitLeave(AstNode node);
    }
}
