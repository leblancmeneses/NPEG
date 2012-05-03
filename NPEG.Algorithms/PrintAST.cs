using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NPEG.Algorithms
{
    public class PrintAST : IAstNodeReplacement
    {
        public PrintAST() 
        {
        }

        public override void VisitEnter(AstNode node)
        {
            System.Diagnostics.Debug.WriteLine("VisitEnter: " + node.Token.Name + " " + ((node.Children.Count == 0) ? node.Token.Value : "") );
            System.Diagnostics.Debug.Indent();
        }

        public override void VisitLeave(AstNode node)
        {
            System.Diagnostics.Debug.Unindent();
            System.Diagnostics.Debug.WriteLine("VisitLeave: " + node.Token.Name );
        }
    }
}
