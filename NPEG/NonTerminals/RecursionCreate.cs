using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace NPEG.NonTerminals
{
    [DataContract]
    public class RecursionCreate : AComposite
    {      
        private AExpression exp;

        public RecursionCreate(String unique, AExpression exp)
        {
            this.FunctionName = unique;
            this.exp = exp;
        }

        [DataMember]
        public String FunctionName
        {
            get;
            set;
        }

        // type cannot be serialized in wcf service
        public Type TypeContains
        {
            get { return this.exp.GetType(); }
        }

        [DataMember]
        public override List<AExpression> Children
        {
            get { return new List<AExpression>() { exp }; }
        }
    }
}
