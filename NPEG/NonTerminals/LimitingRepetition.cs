using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace NPEG.NonTerminals
{
    //e{min,max}                // Match input at least min times but not more than max times against e. 
    //e{,max}                   // Match input at zero or more times but not more than max times against e. 
    //e{min,}                   // Match input at least min times against e. (no limit on max)
    //e{exactcount}             // Match input a total of exactcount agaist e.
    [DataContract]
    public class LimitingRepetition : AComposite
    {        
        private AExpression Exp;
        public LimitingRepetition(AExpression exp)
        {
            this.Exp = exp;
        }

        [DataMember]
        public Int32? Min
        {
            get;
            set;
        }

        [DataMember]
        public Int32? Max
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
