using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace NPEG
{
	[DataContract]
	[DebuggerDisplay("TokenMatch: {Name} = ({Start},{End})")]
	public class TokenMatch
	{
		public TokenMatch(String name, Int32 start, Int32 end)
		{
			Name = name;
			Start = start;
			End = end;
		}

		[DataMember]
		public String Name { private set; get; }

		[DataMember]
		public Int32 Start { private set; get; }

		[DataMember]
		public Int32 End { private set; get; }
	}
}