using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RobustHaven.Text.Npeg
{
    abstract public class InputIterator
    {
        public InputIterator(Int32 length)
        {
            this.Index = 0;
            this.Length = length;
        }

        public Int32 Length
        {
            get;
            private set;
        }
        public Int32 Index;


        abstract public Byte[] Text(Int32 start, Int32 end);



        // Instead of returning a Byte
        // an Int16 is used so error condition is encoded with value.
        // if the sign bit is flagged we know an error occurred.
        // 100000000 00000000 == -1
        // if sign bit is not enabled 
        // we know casting (Byte)int16value will provide valid data of stream.
        // Boolean MoveNext()
        // Boolean MoveBack()
        // Boolean IsCurrentValid() used inconjuction with Byte Current()

        abstract public Int16 Current();
        abstract public Int16 Next();
        abstract public Int16 Previous();
    }
}
