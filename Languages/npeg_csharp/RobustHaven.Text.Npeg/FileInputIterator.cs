using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace RobustHaven.Text.Npeg
{
    public class FileInputIterator : InputIterator
    {
        private System.IO.Stream input;
        public FileInputIterator(System.IO.Stream input)
            : base((Int32)input.Length)
        {
            this.input = input;
        }

        public override byte[] Text(int start, int end)
        {
            byte[] buffer = new byte[end - start];
            this.input.Read(buffer, start, end - start);
            return buffer;
        }

        public override short Current()
        {
            if (this.Index >= this.Length) return -1;
            else
            {
                return this.GetByte(this.Index);
            }
        }

        public override short Next()
        {
            if (this.Index >= this.Length) return -1;
            else
            {
                this.Index += 1;

                if (this.Index >= this.Length) return -1;
                else return this.GetByte(this.Index); ;
            }
        }

        public override short Previous()
        {
            if (this.Index <= 0)
            {
                return -1;
            }
            else
            {
                Debug.Assert(this.Length > 0);

                this.Index -= 1;

                return this.GetByte(this.Index); ;
            }
        }



        private short GetByte(Int32 index)
        {
            Byte[] b = new Byte[1];
            this.input.Read(b, this.Index, 1);
            return b[0];
        }
    }
}
