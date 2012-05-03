using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using NPEG.NonTerminals;
using NPEG.Terminals;

namespace NPEG.Service
{
	//http://www.pluralsight.com/community/blogs/aaron/archive/2006/04/21/22284.aspx

	[ServiceContract]
	[ServiceKnownType(typeof (AndPredicate))]
	[ServiceKnownType(typeof (CapturingGroup))]
	[ServiceKnownType(typeof (LimitingRepetition))]
	[ServiceKnownType(typeof (NotPredicate))]
	[ServiceKnownType(typeof (OneOrMore))]
	[ServiceKnownType(typeof (Optional))]
	[ServiceKnownType(typeof (PrioritizedChoice))]
	[ServiceKnownType(typeof (RecursionCreate))]
	[ServiceKnownType(typeof (Sequence))]
	[ServiceKnownType(typeof (ZeroOrMore))]
	[ServiceKnownType(typeof (AnyCharacter))]
	[ServiceKnownType(typeof (CharacterClass))]
	[ServiceKnownType(typeof (CodePoint))]
	[ServiceKnownType(typeof (DynamicBackReference))]
	[ServiceKnownType(typeof (Fatal))]
	[ServiceKnownType(typeof (Literal))]
	[ServiceKnownType(typeof (RecursionCall))]
	//[ServiceKnownType(typeof(Terminals.Warn))] // already included in List<Warn> Warnings
	public interface IParsingService
	{
		[OperationContract] //Byte[]
		NpegData Parse(String rules, Byte[] input);
	}


	[DataContract]
	public class NpegData
	{
		public NpegData()
		{
			Ast = null;
			ParseTree = null;
			Warnings = new List<Warn>();
		}

		[DataMember]
		public AstNode Ast { get; set; }

		[DataMember]
		public AExpression ParseTree { get; set; }

		[DataMember]
		public List<Warn> Warnings { get; set; }
	}
}