using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace NPEG.NonTerminals
{
    [DataContract]
    public class Optional : AComposite
    {        
        private AExpression Exp;
        public Optional(AExpression exp)
        {
            this.Exp = exp;
        }

        [DataMember]
        public override List<AExpression> Children
        {
            get { return new List<AExpression>() { Exp }; }
        }
    }
}
