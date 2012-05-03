using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;


namespace NPEG.GrammarInterpreter
{
    public class StatementAstNode : IAstNodeReplacement
    {
        public StatementAstNode()
        {
        }

        public override void VisitEnter(AstNode node)
        {  
        }

        public override void VisitLeave(AstNode node)
        {
        }


        public String Name
        {
            get
            {
                if (this.IsCaptured)
                {
                    return this.Children[0].Children[0].Children[0].Token.Value;
                }
                else
                {
                    return this.Children[0].Token.Value;
                }
            }
        }

        public Boolean IsCaptured
        {
            get
            {
                if (this.Children[0].Children[0].Token.Name == "TerminalReference")
                {
                    return true;
                }
                else
                {
                    // it's a label and will not be captured
                    return false;
                }
            }
        }
    }
}
