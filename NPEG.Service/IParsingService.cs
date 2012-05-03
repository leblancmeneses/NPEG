using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace NPEG.Service
{
    //http://www.pluralsight.com/community/blogs/aaron/archive/2006/04/21/22284.aspx

    [ServiceContract]

    [ServiceKnownType(typeof(NonTerminals.AndPredicate))]
    [ServiceKnownType(typeof(NonTerminals.CapturingGroup))]
    [ServiceKnownType(typeof(NonTerminals.LimitingRepetition))]
    [ServiceKnownType(typeof(NonTerminals.NotPredicate))]
    [ServiceKnownType(typeof(NonTerminals.OneOrMore))]
    [ServiceKnownType(typeof(NonTerminals.Optional))]
    [ServiceKnownType(typeof(NonTerminals.PrioritizedChoice))]
    [ServiceKnownType(typeof(NonTerminals.RecursionCreate))]
    [ServiceKnownType(typeof(NonTerminals.Sequence))]
    [ServiceKnownType(typeof(NonTerminals.ZeroOrMore))]

    [ServiceKnownType(typeof(Terminals.AnyCharacter))]
    [ServiceKnownType(typeof(Terminals.CharacterClass))]
    [ServiceKnownType(typeof(Terminals.CodePoint))]
    [ServiceKnownType(typeof(Terminals.DynamicBackReference))]
    [ServiceKnownType(typeof(Terminals.Fatal))]
    [ServiceKnownType(typeof(Terminals.Literal))]
    [ServiceKnownType(typeof(Terminals.RecursionCall))]
    //[ServiceKnownType(typeof(Terminals.Warn))] // already included in List<Warn> Warnings
    public interface IParsingService
    {
        [OperationContract]//Byte[]
        NpegData Parse(String rules, Byte[] input);
    }


    [DataContract]
    public class NpegData
    {
        public NpegData()
        {
            this.Ast = null;
            this.ParseTree = null;
            this.Warnings = new List<NPEG.Terminals.Warn>();
        }

        [DataMember]
        public AstNode Ast
        {
            get;
            set;
        }

        [DataMember]
        public AExpression ParseTree
        {
            get;
            set;
        }

        [DataMember]
        public List<Terminals.Warn> Warnings
        {
            get;
            set;
        }
    }
}
