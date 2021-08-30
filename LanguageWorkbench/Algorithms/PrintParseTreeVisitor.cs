using NPEG;
using NPEG.NonTerminals;
using NPEG.Terminals;

namespace LanguageWorkbench.Algorithms
{
	public class PrintParseTreeVisitor : IParseTreeVisitor
	{
		readonly IndentedStringBuilder _sb = new IndentedStringBuilder();

		public PrintParseTreeVisitor()
		{
			_sb.AppendLine("");
			_sb.AppendLine("");
			_sb.AppendLine("Start - CompositePrintParseTreeVisitor");
		}

		public override string ToString()
		{
			return _sb.ToString();
		}

		public void Dispose()
		{
			_sb.AppendLine("End   - CompositePrintParseTreeVisitor");
			_sb.AppendLine("");
			_sb.AppendLine("");
		}

		public override void Visit(AnyCharacter expression)
		{
			_sb.AppendLine("Visit: AnyCharacter");
		}

		public override void Visit(CharacterClass expression)
		{
			_sb.AppendLine("Visit: CharacterClass; " + expression.ClassExpression);
		}

		public override void Visit(Literal expression)
		{
			_sb.AppendLine("Visit: Literal; " + expression.MatchText);
		}

		public override void Visit(RecursionCall expression)
		{
			_sb.AppendLine("Visit: RecursionCall");
		}

		public override void Visit(DynamicBackReference expression)
		{
			_sb.AppendLine("Visit: DynamicBackReference");
		}

		public override void Visit(CodePoint expression)
		{
			_sb.AppendLine("Visit: CodePoint");
		}

		public override void Visit(Warn expression)
		{
			_sb.AppendLine("Visit: Warn - " + expression.Message);
		}

		public override void Visit(Fatal expression)
		{
			_sb.AppendLine("Visit: Fatal - " + expression.Message);
		}


		public override void VisitEnter(CapturingGroup expression)
		{
			_sb.AppendLine("VisitEnter: CapturingGroup " + expression.Name);
			_sb.IncreaseIndent();
		}

		public override void VisitExecute(CapturingGroup expression)
		{
			_sb.AppendLine("VisitExecute: CapturingGroup");
		}

		public override void VisitLeave(CapturingGroup expression)
		{
			_sb.DecreaseIndent();
			_sb.AppendLine("VisitLeave: CapturingGroup " + expression.Name);
		}


		public override void VisitEnter(RecursionCreate expression)
		{
			_sb.AppendLine("VisitEnter: RecursionCreate " + expression.FunctionName);
			_sb.IncreaseIndent();
		}

		public override void VisitExecute(RecursionCreate expression)
		{
			_sb.AppendLine("VisitExecute: RecursionCreate");
		}

		public override void VisitLeave(RecursionCreate expression)
		{
			_sb.DecreaseIndent();
			_sb.AppendLine("VisitLeave: RecursionCreate " + expression.FunctionName);
		}


		public override void VisitEnter(AndPredicate expression)
		{
			_sb.AppendLine("VisitEnter: AndPredicate");
			_sb.IncreaseIndent();
		}

		public override void VisitExecute(AndPredicate expression)
		{
			_sb.AppendLine("VisitExecute: AndPredicate");
		}

		public override void VisitLeave(AndPredicate expression)
		{
			_sb.DecreaseIndent();
			_sb.AppendLine("VisitLeave: AndPredicate");
		}


		public override void VisitEnter(NotPredicate expression)
		{
			_sb.AppendLine("VisitEnter: NotPredicate");
			_sb.IncreaseIndent();
		}

		public override void VisitExecute(NotPredicate expression)
		{
			_sb.AppendLine("VisitExecute: NotPredicate");
		}

		public override void VisitLeave(NotPredicate expression)
		{
			_sb.DecreaseIndent();
			_sb.AppendLine("VisitLeave: NotPredicate");
		}


		public override void VisitEnter(OneOrMore expression)
		{
			_sb.AppendLine("VisitEnter: OneOrMore");
			_sb.IncreaseIndent();
		}

		public override void VisitExecute(OneOrMore expression)
		{
			_sb.AppendLine("VisitExecute: OneOrMore");
		}

		public override void VisitLeave(OneOrMore expression)
		{
			_sb.DecreaseIndent();
			_sb.AppendLine("VisitLeave: OneOrMore");
		}


		public override void VisitEnter(Optional expression)
		{
			_sb.AppendLine("VisitEnter: Optional");
			_sb.IncreaseIndent();
		}

		public override void VisitExecute(Optional expression)
		{
			_sb.AppendLine("VisitExecute: Optional");
		}

		public override void VisitLeave(Optional expression)
		{
			_sb.DecreaseIndent();
			_sb.AppendLine("VisitLeave: Optional");
		}


		public override void VisitEnter(ZeroOrMore expression)
		{
			_sb.AppendLine("VisitEnter: ZeroOrMore");
			_sb.IncreaseIndent();
		}

		public override void VisitExecute(ZeroOrMore expression)
		{
			_sb.AppendLine("VisitExecute: ZeroOrMore");
		}

		public override void VisitLeave(ZeroOrMore expression)
		{
			_sb.DecreaseIndent();
			_sb.AppendLine("VisitLeave: ZeroOrMore");
		}


		public override void VisitEnter(LimitingRepetition expression)
		{
			_sb.AppendLine("VisitEnter: LimitingRepetition");
			_sb.IncreaseIndent();
		}

		public override void VisitExecute(LimitingRepetition expression)
		{
			_sb.AppendLine("VisitExecute: LimitingRepetition");
		}

		public override void VisitLeave(LimitingRepetition expression)
		{
			_sb.DecreaseIndent();
			_sb.AppendLine("VisitLeave: LimitingRepetition");
		}


		public override void VisitEnter(PrioritizedChoice expression)
		{
			_sb.AppendLine("VisitEnter: PrioritizedChoice");
			_sb.IncreaseIndent();
		}

		public override void VisitExecute(PrioritizedChoice expression)
		{
			_sb.AppendLine("VisitExecute: PrioritizedChoice");
		}

		public override void VisitLeave(PrioritizedChoice expression)
		{
			_sb.DecreaseIndent();
			_sb.AppendLine("VisitLeave: PrioritizedChoice");
		}


		public override void VisitEnter(Sequence expression)
		{
			_sb.AppendLine("VisitEnter: Sequence");
			_sb.IncreaseIndent();
		}

		public override void VisitExecute(Sequence expression)
		{
			_sb.AppendLine("VisitExecute: Sequence");
		}

		public override void VisitLeave(Sequence expression)
		{
			_sb.DecreaseIndent();
			_sb.AppendLine("VisitLeave: Sequence");
		}
	}
}