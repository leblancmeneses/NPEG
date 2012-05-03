using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace NPEG.Terminals
{
    [DataContract]
    public class Fatal : ALeaf
    {
        public Fatal()
            : base()
        {
            this.Message = String.Empty;
        }

        [DataMember]
        public String Message
        {
            get;
            set;
        }
    }
}
