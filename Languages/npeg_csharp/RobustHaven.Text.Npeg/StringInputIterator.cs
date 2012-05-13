using System;
using System.Diagnostics;

namespace RobustHaven.Text.Npeg
{
    public class StringInputIterator : InputIterator
    {
        Byte[] input;

        public StringInputIterator(Byte[] input)
            : base(input.Length)
        {
            this.input = input;
        }

        public StringInputIterator(String us_ascii)
            : base(System.Text.Encoding.ASCII.GetBytes(us_ascii).Length)
        {
            this.input = System.Text.Encoding.ASCII.GetBytes(us_ascii);
            this.Index = 0;
        }


        public override byte[] Text(int start, int end)
        {
            byte[] bytes = new byte[end - start];
            Array.Copy(input, start, bytes, 0, end - start);
            return bytes;
        }


        public override short Current()
        {  
            if (this.Index >= this.Length) return -1;
            else {
                return this.input[this.Index];
            }
        }

        public override short Next()
        {
            if (this.Index >= this.Length) return -1;
            else {
                this.Index += 1;

                if (this.Index >= this.Length) return -1;
                else return this.input[this.Index];
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

                return this.input[this.Index];
            }
        }
    }
}
