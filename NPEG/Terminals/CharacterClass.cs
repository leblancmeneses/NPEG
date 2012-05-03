using System;
using System.Runtime.Serialization;

namespace NPEG.Terminals
{
	[DataContract]
	public class CharacterClass : ALeaf
	{
		[DataMember]
		public String ClassExpression { get; set; }
	}
}