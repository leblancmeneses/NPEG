using System;

namespace RobustHaven.Text.Npeg.Tests.Parsers
{
	public class MathematicalFormula : NpegParser
	{
		public MathematicalFormula(InputIterator iterator) : base(iterator)
		{
		}

		public override Boolean IsMatch()
		{
			return MathematicalFormula_impl_0();
		}


		protected Boolean MathematicalFormula_impl_0()
		{
			return MathematicalFormula_impl_1();
		}

		protected Boolean MathematicalFormula_impl_1()
		{
			String _nodeName_0 = "Expr";
			return CapturingGroup(MathematicalFormula_impl_2, _nodeName_0, false, false);
		}

		protected Boolean MathematicalFormula_impl_2()
		{
			String _nodeName_0 = "Sum";
			return CapturingGroup(MathematicalFormula_impl_3, _nodeName_0, false, false);
		}

		protected Boolean MathematicalFormula_impl_3()
		{
			return Sequence(MathematicalFormula_impl_4, MathematicalFormula_impl_30);
		}

		protected Boolean MathematicalFormula_impl_30()
		{
			return ZeroOrMore(MathematicalFormula_impl_31, "");
		}

		protected Boolean MathematicalFormula_impl_31()
		{
			return Sequence(MathematicalFormula_impl_32, MathematicalFormula_impl_36);
		}

		protected Boolean MathematicalFormula_impl_36()
		{
			String _nodeName_0 = "Product";
			return CapturingGroup(MathematicalFormula_impl_37, _nodeName_0, false, false);
		}

		protected Boolean MathematicalFormula_impl_37()
		{
			return Sequence(MathematicalFormula_impl_38, MathematicalFormula_impl_47);
		}

		protected Boolean MathematicalFormula_impl_47()
		{
			return ZeroOrMore(MathematicalFormula_impl_48, "");
		}

		protected Boolean MathematicalFormula_impl_48()
		{
			return Sequence(MathematicalFormula_impl_49, MathematicalFormula_impl_53);
		}

		protected Boolean MathematicalFormula_impl_53()
		{
			String _nodeName_0 = "Value";
			return CapturingGroup(MathematicalFormula_impl_54, _nodeName_0, false, false);
		}

		protected Boolean MathematicalFormula_impl_54()
		{
			return PrioritizedChoice(MathematicalFormula_impl_55, MathematicalFormula_impl_57);
		}

		protected Boolean MathematicalFormula_impl_57()
		{
			return Sequence(MathematicalFormula_impl_58, MathematicalFormula_impl_61);
		}

		protected Boolean MathematicalFormula_impl_61()
		{
			String _literal_0 = ")";
			return Literal(_literal_0, true);
		}

		protected Boolean MathematicalFormula_impl_58()
		{
			return Sequence(MathematicalFormula_impl_59, MathematicalFormula_impl_60);
		}

		protected Boolean MathematicalFormula_impl_60()
		{
			return RecursionCall(MathematicalFormula_impl_0);
		}

		protected Boolean MathematicalFormula_impl_59()
		{
			String _literal_0 = "(";
			return Literal(_literal_0, true);
		}

		protected Boolean MathematicalFormula_impl_55()
		{
			return OneOrMore(MathematicalFormula_impl_56, "");
		}

		protected Boolean MathematicalFormula_impl_56()
		{
			String _classExpression_0 = "[0-9]";
			return CharacterClass(_classExpression_0, 5);
		}

		protected Boolean MathematicalFormula_impl_49()
		{
			String _nodeName_0 = "Symbol";
			return CapturingGroup(MathematicalFormula_impl_50, _nodeName_0, false, false);
		}

		protected Boolean MathematicalFormula_impl_50()
		{
			return PrioritizedChoice(MathematicalFormula_impl_51, MathematicalFormula_impl_52);
		}

		protected Boolean MathematicalFormula_impl_52()
		{
			String _literal_0 = "/";
			return Literal(_literal_0, true);
		}

		protected Boolean MathematicalFormula_impl_51()
		{
			String _literal_0 = "*";
			return Literal(_literal_0, true);
		}

		protected Boolean MathematicalFormula_impl_38()
		{
			String _nodeName_0 = "Value";
			return CapturingGroup(MathematicalFormula_impl_39, _nodeName_0, false, false);
		}

		protected Boolean MathematicalFormula_impl_39()
		{
			return PrioritizedChoice(MathematicalFormula_impl_40, MathematicalFormula_impl_42);
		}

		protected Boolean MathematicalFormula_impl_42()
		{
			return Sequence(MathematicalFormula_impl_43, MathematicalFormula_impl_46);
		}

		protected Boolean MathematicalFormula_impl_46()
		{
			String _literal_0 = ")";
			return Literal(_literal_0, true);
		}

		protected Boolean MathematicalFormula_impl_43()
		{
			return Sequence(MathematicalFormula_impl_44, MathematicalFormula_impl_45);
		}

		protected Boolean MathematicalFormula_impl_45()
		{
			return RecursionCall(MathematicalFormula_impl_0);
		}

		protected Boolean MathematicalFormula_impl_44()
		{
			String _literal_0 = "(";
			return Literal(_literal_0, true);
		}

		protected Boolean MathematicalFormula_impl_40()
		{
			return OneOrMore(MathematicalFormula_impl_41, "");
		}

		protected Boolean MathematicalFormula_impl_41()
		{
			String _classExpression_0 = "[0-9]";
			return CharacterClass(_classExpression_0, 5);
		}

		protected Boolean MathematicalFormula_impl_32()
		{
			String _nodeName_0 = "Symbol";
			return CapturingGroup(MathematicalFormula_impl_33, _nodeName_0, false, false);
		}

		protected Boolean MathematicalFormula_impl_33()
		{
			return PrioritizedChoice(MathematicalFormula_impl_34, MathematicalFormula_impl_35);
		}

		protected Boolean MathematicalFormula_impl_35()
		{
			String _literal_0 = "-";
			return Literal(_literal_0, true);
		}

		protected Boolean MathematicalFormula_impl_34()
		{
			String _literal_0 = "+";
			return Literal(_literal_0, true);
		}

		protected Boolean MathematicalFormula_impl_4()
		{
			String _nodeName_0 = "Product";
			return CapturingGroup(MathematicalFormula_impl_5, _nodeName_0, false, false);
		}

		protected Boolean MathematicalFormula_impl_5()
		{
			return Sequence(MathematicalFormula_impl_6, MathematicalFormula_impl_15);
		}

		protected Boolean MathematicalFormula_impl_15()
		{
			return ZeroOrMore(MathematicalFormula_impl_16, "");
		}

		protected Boolean MathematicalFormula_impl_16()
		{
			return Sequence(MathematicalFormula_impl_17, MathematicalFormula_impl_21);
		}

		protected Boolean MathematicalFormula_impl_21()
		{
			String _nodeName_0 = "Value";
			return CapturingGroup(MathematicalFormula_impl_22, _nodeName_0, false, false);
		}

		protected Boolean MathematicalFormula_impl_22()
		{
			return PrioritizedChoice(MathematicalFormula_impl_23, MathematicalFormula_impl_25);
		}

		protected Boolean MathematicalFormula_impl_25()
		{
			return Sequence(MathematicalFormula_impl_26, MathematicalFormula_impl_29);
		}

		protected Boolean MathematicalFormula_impl_29()
		{
			String _literal_0 = ")";
			return Literal(_literal_0, true);
		}

		protected Boolean MathematicalFormula_impl_26()
		{
			return Sequence(MathematicalFormula_impl_27, MathematicalFormula_impl_28);
		}

		protected Boolean MathematicalFormula_impl_28()
		{
			return RecursionCall(MathematicalFormula_impl_0);
		}

		protected Boolean MathematicalFormula_impl_27()
		{
			String _literal_0 = "(";
			return Literal(_literal_0, true);
		}

		protected Boolean MathematicalFormula_impl_23()
		{
			return OneOrMore(MathematicalFormula_impl_24, "");
		}

		protected Boolean MathematicalFormula_impl_24()
		{
			String _classExpression_0 = "[0-9]";
			return CharacterClass(_classExpression_0, 5);
		}

		protected Boolean MathematicalFormula_impl_17()
		{
			String _nodeName_0 = "Symbol";
			return CapturingGroup(MathematicalFormula_impl_18, _nodeName_0, false, false);
		}

		protected Boolean MathematicalFormula_impl_18()
		{
			return PrioritizedChoice(MathematicalFormula_impl_19, MathematicalFormula_impl_20);
		}

		protected Boolean MathematicalFormula_impl_20()
		{
			String _literal_0 = "/";
			return Literal(_literal_0, true);
		}

		protected Boolean MathematicalFormula_impl_19()
		{
			String _literal_0 = "*";
			return Literal(_literal_0, true);
		}

		protected Boolean MathematicalFormula_impl_6()
		{
			String _nodeName_0 = "Value";
			return CapturingGroup(MathematicalFormula_impl_7, _nodeName_0, false, false);
		}

		protected Boolean MathematicalFormula_impl_7()
		{
			return PrioritizedChoice(MathematicalFormula_impl_8, MathematicalFormula_impl_10);
		}

		protected Boolean MathematicalFormula_impl_10()
		{
			return Sequence(MathematicalFormula_impl_11, MathematicalFormula_impl_14);
		}

		protected Boolean MathematicalFormula_impl_14()
		{
			String _literal_0 = ")";
			return Literal(_literal_0, true);
		}

		protected Boolean MathematicalFormula_impl_11()
		{
			return Sequence(MathematicalFormula_impl_12, MathematicalFormula_impl_13);
		}

		protected Boolean MathematicalFormula_impl_13()
		{
			return RecursionCall(MathematicalFormula_impl_0);
		}

		protected Boolean MathematicalFormula_impl_12()
		{
			String _literal_0 = "(";
			return Literal(_literal_0, true);
		}

		protected Boolean MathematicalFormula_impl_8()
		{
			return OneOrMore(MathematicalFormula_impl_9, "");
		}

		protected Boolean MathematicalFormula_impl_9()
		{
			String _classExpression_0 = "[0-9]";
			return CharacterClass(_classExpression_0, 5);
		}
	}
}