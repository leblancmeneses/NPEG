using System;
using System.Runtime.Serialization;

namespace NPEG.Terminals
{
	[DataContract]
	public class Warn : ALeaf
	{
		public Warn()
		{
			Message = String.Empty;
		}

		[DataMember]
		public String Message { get; set; }

		[DataMember]
		public Int32 Position { get; set; }
	}
}