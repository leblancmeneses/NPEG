using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace NPEG.NonTerminals
{
    [DataContract]
    public class NotPredicate : AComposite
    {
        private AExpression Exp;
        public NotPredicate(AExpression exp)
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
