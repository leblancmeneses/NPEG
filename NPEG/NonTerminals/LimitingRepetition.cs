using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NPEG.NonTerminals
{
	//e{min,max}                // Match input at least min times but not more than max times against e. 
	//e{,max}                   // Match input at zero or more times but not more than max times against e. 
	//e{min,}                   // Match input at least min times against e. (no limit on max)
	//e{exactcount}             // Match input a total of exactcount agaist e.
	//e{(\k<C2> - \k<C1>)+1}    // math expression to support variable length protocols using backreferencing
	[DataContract]
	public class LimitingRepetition : AComposite
	{
		private readonly AExpression Exp;

		public LimitingRepetition(AExpression exp)
		{
			Exp = exp;
			Min = null;
			Max = null;
			VariableLengthExpression = null;
		}

		[DataMember]
		public Int32? Min { get; set; }

		[DataMember]
		public Int32? Max { get; set; }

		public string VariableLengthExpression { get; set; }

		[DataMember]
		public override List<AExpression> Children
		{
			get { return new List<AExpression> {Exp}; }
		}
	}
}