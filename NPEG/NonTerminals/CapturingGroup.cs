using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace NPEG.NonTerminals
{
    [DataContract]
    public class CapturingGroup : AComposite
    {
        private AExpression Exp;

        public CapturingGroup(String uniquename, AExpression exp)
        {
            this.Exp = exp;
            this.Name = uniquename;
            this.DoReplaceBySingleChildNode = false;
            this.DoCreateCustomAstNode = false;
        }

        [DataMember]
        public String Name
        {
            get;
            set;
        }

        [DataMember]
        public Boolean DoReplaceBySingleChildNode 
        {
            get;
            set;
        }

        [DataMember]
        public Boolean DoCreateCustomAstNode
        {
            get;
            set;
        }


        [DataMember]
        public override List<AExpression> Children
        {
            get { return new List<AExpression>() { Exp }; }
        }
    }
}
