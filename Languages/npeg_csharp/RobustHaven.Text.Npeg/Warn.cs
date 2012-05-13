using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RobustHaven.Text.Npeg
{
    public class Warn
    {
        public Warn()
            : base()
        {
            this.Message = String.Empty;
        }

        public Warn(String message, Int32 position)
            : base()
        {
            this.Message = message;
            this.Position = position;
        }

        public String Message
        {
            get;
            set;
        }

        public Int32 Position
        {
            get;
            set;
        }
    }
}
