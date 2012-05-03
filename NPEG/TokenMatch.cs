using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace NPEG
{
	[DataContract]
	[DebuggerDisplay("TokenMatch: {Name} = {Value}")]
	public class TokenMatch
	{
		public TokenMatch(String name, String value, Int32 start, Int32 end)
		{
			Name = name;
			Value = value;
			Start = start;
			End = end;
		}

		[DataMember]
		public String Name { private set; get; }

		[DataMember]
		public String Value { private set; get; }

		[DataMember]
		public Int32 Start { private set; get; }

		[DataMember]
		public Int32 End { private set; get; }
	}
}