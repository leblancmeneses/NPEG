using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace NPEG.Terminals
{
    [DataContract]
    public class Warn : ALeaf
    {
        public Warn()
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

        [DataMember]
        public Int32 Position
        {
            get;
            set;
        }
    }
}
