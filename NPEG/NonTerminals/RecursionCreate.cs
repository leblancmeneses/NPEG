using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NPEG.NonTerminals
{
	[DataContract]
	public class RecursionCreate : AComposite
	{
		private readonly AExpression exp;

		public RecursionCreate(String unique, AExpression exp)
		{
			FunctionName = unique;
			this.exp = exp;
		}

		[DataMember]
		public String FunctionName { get; set; }

		// type cannot be serialized in wcf service
		public Type TypeContains
		{
			get { return exp.GetType(); }
		}

		[DataMember]
		public override List<AExpression> Children
		{
			get { return new List<AExpression> {exp}; }
		}
	}
}