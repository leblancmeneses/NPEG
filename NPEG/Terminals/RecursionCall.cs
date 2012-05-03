using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace NPEG.Terminals
{
    [DataContract]
    public class RecursionCall : ALeaf
    {
        public RecursionCall(String bindunique)
        {
            this.FunctionName = bindunique;
        }

        [DataMember]
        public String FunctionName
        {
            get;
            set;
        }
    }
}
