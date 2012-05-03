using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace NPEG.NonTerminals
{
    [DataContract]
    public class Sequence : AComposite
    {        
        private AExpression left;
        private AExpression right;

        public Sequence(AExpression left, AExpression right)
        {
            this.left = left;
            this.right = right;
        }

        [DataMember]
        public override List<AExpression> Children
        {
            get { return new List<AExpression>() { left, right }; }
        }
    }
}
