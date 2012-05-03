using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace NPEG.Terminals
{
    [DataContract]
    public class Literal : ALeaf
    {
        public Literal():base()
        {
            this.IsCaseSensitive = true;
        }

        [DataMember]
        public String MatchText
        {
            get;
            set;
        }

        [DataMember]
        public Boolean IsCaseSensitive
        {
            get;
            set;
        }
    }
}
