using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace NPEG.Terminals
{
    [DataContract]
    public class CharacterClass : ALeaf
    {
        [DataMember]
        public String ClassExpression
        {
            get;
            set;
        }
    }
}
