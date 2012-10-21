using System;

namespace RobustHaven.Text.Npeg
{
	public class TokenMatch
	{
		public TokenMatch(String name, Int32 start, Int32 end, Byte[] value)
		{
			Name = name;
			Start = start;
			End = end;
			Value = value;
		}

		public String Name { private set; get; }
		public Int32 Start { private set; get; }

		public Int32 End { private set; get; }

		public Byte[] Value { get; set; }
	}
}