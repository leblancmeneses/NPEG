using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RobustHaven.Text.Npeg
{
    public class TokenMatch
    {
        public TokenMatch(String name, Int32 start, Int32 end, Byte[] value)
        {
            this.Name = name;
            this.Start = start;
            this.End = end;
            this.Value = value;
        }

        public String Name
        {
            private set;
            get;
        }
        public Int32 Start
        {
            private set;
            get;
        }

        public Int32 End
        {
            private set;
            get;
        }

        public Byte[] Value
        {
            get; 
            set;
        }
    }
}
