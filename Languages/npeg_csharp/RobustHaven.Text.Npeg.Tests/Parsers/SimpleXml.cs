using System;

namespace RobustHaven.Text.Npeg.Tests.Parsers
{
	public class SimpleXml : NpegParser
	{
		public SimpleXml(InputIterator iterator) : base(iterator)
		{
		}

		public override Boolean IsMatch()
		{
			return SimpleXml_impl_0();
		}


		protected Boolean SimpleXml_impl_0()
		{
			String _nodeName_0 = "Expression";
			return CapturingGroup(SimpleXml_impl_1, _nodeName_0, false, false);
		}

		protected Boolean SimpleXml_impl_1()
		{
			return OneOrMore(SimpleXml_impl_2, "");
		}

		protected Boolean SimpleXml_impl_2()
		{
			return Sequence(SimpleXml_impl_3, SimpleXml_impl_23);
		}

		protected Boolean SimpleXml_impl_23()
		{
			String _nodeName_0 = "END_TAG";
			return CapturingGroup(SimpleXml_impl_24, _nodeName_0, false, false);
		}

		protected Boolean SimpleXml_impl_24()
		{
			return Sequence(SimpleXml_impl_25, SimpleXml_impl_28);
		}

		protected Boolean SimpleXml_impl_28()
		{
			String _literal_0 = ">";
			return Literal(_literal_0, true);
		}

		protected Boolean SimpleXml_impl_25()
		{
			return Sequence(SimpleXml_impl_26, SimpleXml_impl_27);
		}

		protected Boolean SimpleXml_impl_27()
		{
			String _dynamicBackReference_0 = "TAG";
			return DynamicBackReference(_dynamicBackReference_0, true);
		}

		protected Boolean SimpleXml_impl_26()
		{
			String _literal_0 = "</";
			return Literal(_literal_0, true);
		}

		protected Boolean SimpleXml_impl_3()
		{
			return Sequence(SimpleXml_impl_4, SimpleXml_impl_12);
		}

		protected Boolean SimpleXml_impl_12()
		{
			String _nodeName_0 = "Body";
			return CapturingGroup(SimpleXml_impl_13, _nodeName_0, false, false);
		}

		protected Boolean SimpleXml_impl_13()
		{
			return ZeroOrMore(SimpleXml_impl_14, "");
		}

		protected Boolean SimpleXml_impl_14()
		{
			return Sequence(SimpleXml_impl_15, SimpleXml_impl_22);
		}

		protected Boolean SimpleXml_impl_22()
		{
			return AnyCharacter();
		}

		protected Boolean SimpleXml_impl_15()
		{
			return NotPredicate(SimpleXml_impl_16);
		}

		protected Boolean SimpleXml_impl_16()
		{
			String _nodeName_0 = "END_TAG";
			return CapturingGroup(SimpleXml_impl_17, _nodeName_0, false, false);
		}

		protected Boolean SimpleXml_impl_17()
		{
			return Sequence(SimpleXml_impl_18, SimpleXml_impl_21);
		}

		protected Boolean SimpleXml_impl_21()
		{
			String _literal_0 = ">";
			return Literal(_literal_0, true);
		}

		protected Boolean SimpleXml_impl_18()
		{
			return Sequence(SimpleXml_impl_19, SimpleXml_impl_20);
		}

		protected Boolean SimpleXml_impl_20()
		{
			String _dynamicBackReference_0 = "TAG";
			return DynamicBackReference(_dynamicBackReference_0, true);
		}

		protected Boolean SimpleXml_impl_19()
		{
			String _literal_0 = "</";
			return Literal(_literal_0, true);
		}

		protected Boolean SimpleXml_impl_4()
		{
			String _nodeName_0 = "START_TAG";
			return CapturingGroup(SimpleXml_impl_5, _nodeName_0, false, false);
		}

		protected Boolean SimpleXml_impl_5()
		{
			return Sequence(SimpleXml_impl_6, SimpleXml_impl_11);
		}

		protected Boolean SimpleXml_impl_11()
		{
			String _literal_0 = ">";
			return Literal(_literal_0, true);
		}

		protected Boolean SimpleXml_impl_6()
		{
			return Sequence(SimpleXml_impl_7, SimpleXml_impl_8);
		}

		protected Boolean SimpleXml_impl_8()
		{
			String _nodeName_0 = "TAG";
			return CapturingGroup(SimpleXml_impl_9, _nodeName_0, false, false);
		}

		protected Boolean SimpleXml_impl_9()
		{
			return OneOrMore(SimpleXml_impl_10, "");
		}

		protected Boolean SimpleXml_impl_10()
		{
			String _classExpression_0 = "[a-zA-Z0-9]";
			return CharacterClass(_classExpression_0, 11);
		}

		protected Boolean SimpleXml_impl_7()
		{
			String _literal_0 = "<";
			return Literal(_literal_0, true);
		}
	}
}