using System;
using System.Runtime.Serialization;

namespace NPEG.Terminals
{
	[DataContract]
	public class Literal : ALeaf
	{
		public Literal()
		{
			IsCaseSensitive = true;
		}

		[DataMember]
		public String MatchText { get; set; }

		[DataMember]
		public Boolean IsCaseSensitive { get; set; }
	}
}