using System;
using System.Text;
using RobustHaven.Text.Npeg;

namespace RobustHaven.Text.Npeg.Tests.Parsers
{
	public class MathematicalFormula : NpegParser
	{
		public MathematicalFormula(InputIterator iterator): base(iterator){}

		public override Boolean IsMatch()
		{
			return MathematicalFormula_impl_0();
		}


		protected Boolean MathematicalFormula_impl_0()
		{
			return this.MathematicalFormula_impl_1();
		}

		protected Boolean MathematicalFormula_impl_1()
		{
			String _nodeName_0 = "Expr";
			return this.CapturingGroup(new IsMatchPredicate(this.MathematicalFormula_impl_2), _nodeName_0, false, false);
		}

		protected Boolean MathematicalFormula_impl_2()
		{
			String _nodeName_0 = "Sum";
			return this.CapturingGroup(new IsMatchPredicate(this.MathematicalFormula_impl_3), _nodeName_0, false, false);
		}

		protected Boolean MathematicalFormula_impl_3()
		{
			return this.Sequence(new IsMatchPredicate(this.MathematicalFormula_impl_4), new IsMatchPredicate(this.MathematicalFormula_impl_30));
		}

		protected Boolean MathematicalFormula_impl_30()
		{
			return this.ZeroOrMore(new IsMatchPredicate(this.MathematicalFormula_impl_31), "");
		}

		protected Boolean MathematicalFormula_impl_31()
		{
			return this.Sequence(new IsMatchPredicate(this.MathematicalFormula_impl_32), new IsMatchPredicate(this.MathematicalFormula_impl_36));
		}

		protected Boolean MathematicalFormula_impl_36()
		{
			String _nodeName_0 = "Product";
			return this.CapturingGroup(new IsMatchPredicate(this.MathematicalFormula_impl_37), _nodeName_0, false, false);
		}

		protected Boolean MathematicalFormula_impl_37()
		{
			return this.Sequence(new IsMatchPredicate(this.MathematicalFormula_impl_38), new IsMatchPredicate(this.MathematicalFormula_impl_47));
		}

		protected Boolean MathematicalFormula_impl_47()
		{
			return this.ZeroOrMore(new IsMatchPredicate(this.MathematicalFormula_impl_48), "");
		}

		protected Boolean MathematicalFormula_impl_48()
		{
			return this.Sequence(new IsMatchPredicate(this.MathematicalFormula_impl_49), new IsMatchPredicate(this.MathematicalFormula_impl_53));
		}

		protected Boolean MathematicalFormula_impl_53()
		{
			String _nodeName_0 = "Value";
			return this.CapturingGroup(new IsMatchPredicate(this.MathematicalFormula_impl_54), _nodeName_0, false, false);
		}

		protected Boolean MathematicalFormula_impl_54()
		{
			return this.PrioritizedChoice(new IsMatchPredicate(this.MathematicalFormula_impl_55), new IsMatchPredicate(this.MathematicalFormula_impl_57));
		}

		protected Boolean MathematicalFormula_impl_57()
		{
			return this.Sequence(new IsMatchPredicate(this.MathematicalFormula_impl_58), new IsMatchPredicate(this.MathematicalFormula_impl_61));
		}

		protected Boolean MathematicalFormula_impl_61()
		{
			String _literal_0 = ")";
			return this.Literal(_literal_0, true);
		}

		protected Boolean MathematicalFormula_impl_58()
		{
			return this.Sequence(new IsMatchPredicate(this.MathematicalFormula_impl_59), new IsMatchPredicate(this.MathematicalFormula_impl_60));
		}

		protected Boolean MathematicalFormula_impl_60()
		{
			return this.RecursionCall(new IsMatchPredicate(this.MathematicalFormula_impl_0));
		}

		protected Boolean MathematicalFormula_impl_59()
		{
			String _literal_0 = "(";
			return this.Literal(_literal_0, true);
		}

		protected Boolean MathematicalFormula_impl_55()
		{
			return this.OneOrMore(new IsMatchPredicate(this.MathematicalFormula_impl_56), "");
		}

		protected Boolean MathematicalFormula_impl_56()
		{
			String _classExpression_0 = "[0-9]";
			return this.CharacterClass(_classExpression_0, 5);
		}

		protected Boolean MathematicalFormula_impl_49()
		{
			String _nodeName_0 = "Symbol";
			return this.CapturingGroup(new IsMatchPredicate(this.MathematicalFormula_impl_50), _nodeName_0, false, false);
		}

		protected Boolean MathematicalFormula_impl_50()
		{
			return this.PrioritizedChoice(new IsMatchPredicate(this.MathematicalFormula_impl_51), new IsMatchPredicate(this.MathematicalFormula_impl_52));
		}

		protected Boolean MathematicalFormula_impl_52()
		{
			String _literal_0 = "/";
			return this.Literal(_literal_0, true);
		}

		protected Boolean MathematicalFormula_impl_51()
		{
			String _literal_0 = "*";
			return this.Literal(_literal_0, true);
		}

		protected Boolean MathematicalFormula_impl_38()
		{
			String _nodeName_0 = "Value";
			return this.CapturingGroup(new IsMatchPredicate(this.MathematicalFormula_impl_39), _nodeName_0, false, false);
		}

		protected Boolean MathematicalFormula_impl_39()
		{
			return this.PrioritizedChoice(new IsMatchPredicate(this.MathematicalFormula_impl_40), new IsMatchPredicate(this.MathematicalFormula_impl_42));
		}

		protected Boolean MathematicalFormula_impl_42()
		{
			return this.Sequence(new IsMatchPredicate(this.MathematicalFormula_impl_43), new IsMatchPredicate(this.MathematicalFormula_impl_46));
		}

		protected Boolean MathematicalFormula_impl_46()
		{
			String _literal_0 = ")";
			return this.Literal(_literal_0, true);
		}

		protected Boolean MathematicalFormula_impl_43()
		{
			return this.Sequence(new IsMatchPredicate(this.MathematicalFormula_impl_44), new IsMatchPredicate(this.MathematicalFormula_impl_45));
		}

		protected Boolean MathematicalFormula_impl_45()
		{
			return this.RecursionCall(new IsMatchPredicate(this.MathematicalFormula_impl_0));
		}

		protected Boolean MathematicalFormula_impl_44()
		{
			String _literal_0 = "(";
			return this.Literal(_literal_0, true);
		}

		protected Boolean MathematicalFormula_impl_40()
		{
			return this.OneOrMore(new IsMatchPredicate(this.MathematicalFormula_impl_41), "");
		}

		protected Boolean MathematicalFormula_impl_41()
		{
			String _classExpression_0 = "[0-9]";
			return this.CharacterClass(_classExpression_0, 5);
		}

		protected Boolean MathematicalFormula_impl_32()
		{
			String _nodeName_0 = "Symbol";
			return this.CapturingGroup(new IsMatchPredicate(this.MathematicalFormula_impl_33), _nodeName_0, false, false);
		}

		protected Boolean MathematicalFormula_impl_33()
		{
			return this.PrioritizedChoice(new IsMatchPredicate(this.MathematicalFormula_impl_34), new IsMatchPredicate(this.MathematicalFormula_impl_35));
		}

		protected Boolean MathematicalFormula_impl_35()
		{
			String _literal_0 = "-";
			return this.Literal(_literal_0, true);
		}

		protected Boolean MathematicalFormula_impl_34()
		{
			String _literal_0 = "+";
			return this.Literal(_literal_0, true);
		}

		protected Boolean MathematicalFormula_impl_4()
		{
			String _nodeName_0 = "Product";
			return this.CapturingGroup(new IsMatchPredicate(this.MathematicalFormula_impl_5), _nodeName_0, false, false);
		}

		protected Boolean MathematicalFormula_impl_5()
		{
			return this.Sequence(new IsMatchPredicate(this.MathematicalFormula_impl_6), new IsMatchPredicate(this.MathematicalFormula_impl_15));
		}

		protected Boolean MathematicalFormula_impl_15()
		{
			return this.ZeroOrMore(new IsMatchPredicate(this.MathematicalFormula_impl_16), "");
		}

		protected Boolean MathematicalFormula_impl_16()
		{
			return this.Sequence(new IsMatchPredicate(this.MathematicalFormula_impl_17), new IsMatchPredicate(this.MathematicalFormula_impl_21));
		}

		protected Boolean MathematicalFormula_impl_21()
		{
			String _nodeName_0 = "Value";
			return this.CapturingGroup(new IsMatchPredicate(this.MathematicalFormula_impl_22), _nodeName_0, false, false);
		}

		protected Boolean MathematicalFormula_impl_22()
		{
			return this.PrioritizedChoice(new IsMatchPredicate(this.MathematicalFormula_impl_23), new IsMatchPredicate(this.MathematicalFormula_impl_25));
		}

		protected Boolean MathematicalFormula_impl_25()
		{
			return this.Sequence(new IsMatchPredicate(this.MathematicalFormula_impl_26), new IsMatchPredicate(this.MathematicalFormula_impl_29));
		}

		protected Boolean MathematicalFormula_impl_29()
		{
			String _literal_0 = ")";
			return this.Literal(_literal_0, true);
		}

		protected Boolean MathematicalFormula_impl_26()
		{
			return this.Sequence(new IsMatchPredicate(this.MathematicalFormula_impl_27), new IsMatchPredicate(this.MathematicalFormula_impl_28));
		}

		protected Boolean MathematicalFormula_impl_28()
		{
			return this.RecursionCall(new IsMatchPredicate(this.MathematicalFormula_impl_0));
		}

		protected Boolean MathematicalFormula_impl_27()
		{
			String _literal_0 = "(";
			return this.Literal(_literal_0, true);
		}

		protected Boolean MathematicalFormula_impl_23()
		{
			return this.OneOrMore(new IsMatchPredicate(this.MathematicalFormula_impl_24), "");
		}

		protected Boolean MathematicalFormula_impl_24()
		{
			String _classExpression_0 = "[0-9]";
			return this.CharacterClass(_classExpression_0, 5);
		}

		protected Boolean MathematicalFormula_impl_17()
		{
			String _nodeName_0 = "Symbol";
			return this.CapturingGroup(new IsMatchPredicate(this.MathematicalFormula_impl_18), _nodeName_0, false, false);
		}

		protected Boolean MathematicalFormula_impl_18()
		{
			return this.PrioritizedChoice(new IsMatchPredicate(this.MathematicalFormula_impl_19), new IsMatchPredicate(this.MathematicalFormula_impl_20));
		}

		protected Boolean MathematicalFormula_impl_20()
		{
			String _literal_0 = "/";
			return this.Literal(_literal_0, true);
		}

		protected Boolean MathematicalFormula_impl_19()
		{
			String _literal_0 = "*";
			return this.Literal(_literal_0, true);
		}

		protected Boolean MathematicalFormula_impl_6()
		{
			String _nodeName_0 = "Value";
			return this.CapturingGroup(new IsMatchPredicate(this.MathematicalFormula_impl_7), _nodeName_0, false, false);
		}

		protected Boolean MathematicalFormula_impl_7()
		{
			return this.PrioritizedChoice(new IsMatchPredicate(this.MathematicalFormula_impl_8), new IsMatchPredicate(this.MathematicalFormula_impl_10));
		}

		protected Boolean MathematicalFormula_impl_10()
		{
			return this.Sequence(new IsMatchPredicate(this.MathematicalFormula_impl_11), new IsMatchPredicate(this.MathematicalFormula_impl_14));
		}

		protected Boolean MathematicalFormula_impl_14()
		{
			String _literal_0 = ")";
			return this.Literal(_literal_0, true);
		}

		protected Boolean MathematicalFormula_impl_11()
		{
			return this.Sequence(new IsMatchPredicate(this.MathematicalFormula_impl_12), new IsMatchPredicate(this.MathematicalFormula_impl_13));
		}

		protected Boolean MathematicalFormula_impl_13()
		{
			return this.RecursionCall(new IsMatchPredicate(this.MathematicalFormula_impl_0));
		}

		protected Boolean MathematicalFormula_impl_12()
		{
			String _literal_0 = "(";
			return this.Literal(_literal_0, true);
		}

		protected Boolean MathematicalFormula_impl_8()
		{
			return this.OneOrMore(new IsMatchPredicate(this.MathematicalFormula_impl_9), "");
		}

		protected Boolean MathematicalFormula_impl_9()
		{
			String _classExpression_0 = "[0-9]";
			return this.CharacterClass(_classExpression_0, 5);
		}
	}
}
