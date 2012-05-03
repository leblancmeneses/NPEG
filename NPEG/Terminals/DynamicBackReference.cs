using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace NPEG.Terminals
{
    // PEG by nature already support static back referencing.
    // this allows a user to match (, {, /*, and appropriate closing character.
    // however if a user wanted to parse xml DynamicBackReferencing is needed as what the parser must match
    // is not known ahead of time.
    [DataContract]
    public class DynamicBackReference : ALeaf
    {
        public DynamicBackReference()
            : base()
        {
            this.IsCaseSensitive = true;
        }

        [DataMember]
        public String BackReferenceName
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
