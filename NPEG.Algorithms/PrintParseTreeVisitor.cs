using System;
using System.Diagnostics;
using System.Text;
using NPEG.NonTerminals;
using NPEG.Terminals;

namespace NPEG.Algorithms
{
	public class PrintParseTreeVisitor : IParseTreeVisitor
	{
		private readonly StringBuilder tree = new StringBuilder();

		public PrintParseTreeVisitor()
		{
			Debug.WriteLine("");
			Debug.WriteLine("");
			Debug.WriteLine("Start - CompositePrintParseTreeVisitor");
		}

		public String ParseTree
		{
			get { return tree.ToString(); }
		}

		public void Dispose()
		{
			Debug.WriteLine("End   - CompositePrintParseTreeVisitor");
			Debug.WriteLine("");
			Debug.WriteLine("");
		}

		public override void Visit(AnyCharacter expression)
		{
			Debug.WriteLine("Visit: AnyCharacter");
		}

		public override void Visit(CharacterClass expression)
		{
			Debug.WriteLine("Visit: CharacterClass; " + expression.ClassExpression);
		}

		public override void Visit(Literal expression)
		{
			Debug.WriteLine("Visit: Literal; " + expression.MatchText);
		}

		public override void Visit(RecursionCall expression)
		{
			Debug.WriteLine("Visit: RecursionCall");
		}

		public override void Visit(DynamicBackReference expression)
		{
			Debug.WriteLine("Visit: DynamicBackReference");
		}

		public override void Visit(CodePoint expression)
		{
			Debug.WriteLine("Visit: CodePoint");
		}

		public override void Visit(Warn expression)
		{
			Debug.WriteLine("Visit: Warn - " + expression.Message);
		}

		public override void Visit(Fatal expression)
		{
			Debug.WriteLine("Visit: Fatal - " + expression.Message);
		}


		public override void VisitEnter(CapturingGroup expression)
		{
			Debug.WriteLine("VisitEnter: CapturingGroup " + expression.Name);
			Debug.Indent();
		}

		public override void VisitExecute(CapturingGroup expression)
		{
			Debug.WriteLine("VisitExecute: CapturingGroup");
		}

		public override void VisitLeave(CapturingGroup expression)
		{
			Debug.Unindent();
			Debug.WriteLine("VisitLeave: CapturingGroup " + expression.Name);
		}


		public override void VisitEnter(RecursionCreate expression)
		{
			Debug.WriteLine("VisitEnter: RecursionCreate " + expression.FunctionName);
			Debug.Indent();
		}

		public override void VisitExecute(RecursionCreate expression)
		{
			Debug.WriteLine("VisitExecute: RecursionCreate");
		}

		public override void VisitLeave(RecursionCreate expression)
		{
			Debug.Unindent();
			Debug.WriteLine("VisitLeave: RecursionCreate " + expression.FunctionName);
		}


		public override void VisitEnter(AndPredicate expression)
		{
			Debug.WriteLine("VisitEnter: AndPredicate");
			Debug.Indent();
		}

		public override void VisitExecute(AndPredicate expression)
		{
			Debug.WriteLine("VisitExecute: AndPredicate");
		}

		public override void VisitLeave(AndPredicate expression)
		{
			Debug.Unindent();
			Debug.WriteLine("VisitLeave: AndPredicate");
		}


		public override void VisitEnter(NotPredicate expression)
		{
			Debug.WriteLine("VisitEnter: NotPredicate");
			Debug.Indent();
		}

		public override void VisitExecute(NotPredicate expression)
		{
			Debug.WriteLine("VisitExecute: NotPredicate");
		}

		public override void VisitLeave(NotPredicate expression)
		{
			Debug.Unindent();
			Debug.WriteLine("VisitLeave: NotPredicate");
		}


		public override void VisitEnter(OneOrMore expression)
		{
			Debug.WriteLine("VisitEnter: OneOrMore");
			Debug.Indent();
		}

		public override void VisitExecute(OneOrMore expression)
		{
			Debug.WriteLine("VisitExecute: OneOrMore");
		}

		public override void VisitLeave(OneOrMore expression)
		{
			Debug.Unindent();
			Debug.WriteLine("VisitLeave: OneOrMore");
		}


		public override void VisitEnter(Optional expression)
		{
			Debug.WriteLine("VisitEnter: Optional");
			Debug.Indent();
		}

		public override void VisitExecute(Optional expression)
		{
			Debug.WriteLine("VisitExecute: Optional");
		}

		public override void VisitLeave(Optional expression)
		{
			Debug.Unindent();
			Debug.WriteLine("VisitLeave: Optional");
		}


		public override void VisitEnter(ZeroOrMore expression)
		{
			Debug.WriteLine("VisitEnter: ZeroOrMore");
			Debug.Indent();
		}

		public override void VisitExecute(ZeroOrMore expression)
		{
			Debug.WriteLine("VisitExecute: ZeroOrMore");
		}

		public override void VisitLeave(ZeroOrMore expression)
		{
			Debug.Unindent();
			Debug.WriteLine("VisitLeave: ZeroOrMore");
		}


		public override void VisitEnter(LimitingRepetition expression)
		{
			Debug.WriteLine("VisitEnter: LimitingRepetition");
			Debug.Indent();
		}

		public override void VisitExecute(LimitingRepetition expression)
		{
			Debug.WriteLine("VisitExecute: LimitingRepetition");
		}

		public override void VisitLeave(LimitingRepetition expression)
		{
			Debug.Unindent();
			Debug.WriteLine("VisitLeave: LimitingRepetition");
		}


		public override void VisitEnter(PrioritizedChoice expression)
		{
			Debug.WriteLine("VisitEnter: PrioritizedChoice");
			Debug.Indent();
		}

		public override void VisitExecute(PrioritizedChoice expression)
		{
			Debug.WriteLine("VisitExecute: PrioritizedChoice");
		}

		public override void VisitLeave(PrioritizedChoice expression)
		{
			Debug.Unindent();
			Debug.WriteLine("VisitLeave: PrioritizedChoice");
		}


		public override void VisitEnter(Sequence expression)
		{
			Debug.WriteLine("VisitEnter: Sequence");
			Debug.Indent();
		}

		public override void VisitExecute(Sequence expression)
		{
			Debug.WriteLine("VisitExecute: Sequence");
		}

		public override void VisitLeave(Sequence expression)
		{
			Debug.Unindent();
			Debug.WriteLine("VisitLeave: Sequence");
		}
	}
}