using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using NPEG.NonTerminals;

namespace NPEG
{
	[DataContract]
	public abstract class AComposite : AExpression
	{
		#region Predicates

		public AndPredicate And()
		{
			return new AndPredicate(this);
		}

		public NotPredicate Not()
		{
			return new NotPredicate(this);
		}

		#endregion

		#region binary

		public PrioritizedChoice Or(AExpression other)
		{
			return new PrioritizedChoice(this, other);
		}

		public Sequence Sequence(AExpression other)
		{
			return new Sequence(this, other);
		}

		#endregion

		#region unary suffix

		public Optional Optional()
		{
			return new Optional(this);
		}

		public OneOrMore Plus()
		{
			return new OneOrMore(this);
		}

		public ZeroOrMore Star()
		{
			return new ZeroOrMore(this);
		}

		public LimitingRepetition Limit(Int32? min, Int32? max)
		{
			return new LimitingRepetition(this) {Min = min, Max = max};
		}

		#endregion

		[DataMember]
		public abstract List<AExpression> Children { get; }

		public CapturingGroup Capture(String name)
		{
			return new CapturingGroup(name, this);
		}

		// for visitenter, visitexit, visitexecute 
		// require 1 to a maximum of 2 children for them to be called as user would expect.
		public override void Accept(IParseTreeVisitor visitor)
		{
			visitor.VisitEnter(this);

			int i = 0;
			foreach (AExpression expression in Children)
			{
				if (i++ != 0)
				{
					visitor.VisitExecute(this);
				}

				expression.Accept(visitor);
			}

			visitor.VisitLeave(this);
		}
	}
}