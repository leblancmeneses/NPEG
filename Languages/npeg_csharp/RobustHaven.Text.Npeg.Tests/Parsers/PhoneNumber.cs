using System;
using System.Text;
using RobustHaven.Text.Npeg;

namespace RobustHaven.Text.Npeg.Tests.Parsers
{
	public class PhoneNumber : NpegParser
	{
		public PhoneNumber(InputIterator iterator): base(iterator){}

		public override Boolean IsMatch()
		{
			return PhoneNumber_impl_0();
		}


		protected Boolean PhoneNumber_impl_0()
		{
			String _nodeName_0 = "PhoneNumber";
			return this.CapturingGroup(new IsMatchPredicate(this.PhoneNumber_impl_1), _nodeName_0, false, false);
		}

		protected Boolean PhoneNumber_impl_1()
		{
			return this.Sequence(new IsMatchPredicate(this.PhoneNumber_impl_2), new IsMatchPredicate(this.PhoneNumber_impl_13));
		}

		protected Boolean PhoneNumber_impl_13()
		{
			String _nodeName_0 = "FourDigitCode";
			return this.CapturingGroup(new IsMatchPredicate(this.PhoneNumber_impl_14), _nodeName_0, false, false);
		}

		protected Boolean PhoneNumber_impl_14()
		{
			return this.LimitingRepetition(new IsMatchPredicate(this.PhoneNumber_impl_15), 4, 4, "");
		}

		protected Boolean PhoneNumber_impl_15()
		{
			String _classExpression_0 = "[0-9]";
			return this.CharacterClass(_classExpression_0, 5);
		}

		protected Boolean PhoneNumber_impl_2()
		{
			return this.Sequence(new IsMatchPredicate(this.PhoneNumber_impl_3), new IsMatchPredicate(this.PhoneNumber_impl_12));
		}

		protected Boolean PhoneNumber_impl_12()
		{
			String _literal_0 = "-";
			return this.Literal(_literal_0, true);
		}

		protected Boolean PhoneNumber_impl_3()
		{
			return this.Sequence(new IsMatchPredicate(this.PhoneNumber_impl_4), new IsMatchPredicate(this.PhoneNumber_impl_9));
		}

		protected Boolean PhoneNumber_impl_9()
		{
			String _nodeName_0 = "ThreeDigitCode";
			return this.CapturingGroup(new IsMatchPredicate(this.PhoneNumber_impl_10), _nodeName_0, false, false);
		}

		protected Boolean PhoneNumber_impl_10()
		{
			return this.LimitingRepetition(new IsMatchPredicate(this.PhoneNumber_impl_11), 3, 3, "");
		}

		protected Boolean PhoneNumber_impl_11()
		{
			String _classExpression_0 = "[0-9]";
			return this.CharacterClass(_classExpression_0, 5);
		}

		protected Boolean PhoneNumber_impl_4()
		{
			return this.Sequence(new IsMatchPredicate(this.PhoneNumber_impl_5), new IsMatchPredicate(this.PhoneNumber_impl_8));
		}

		protected Boolean PhoneNumber_impl_8()
		{
			String _literal_0 = "-";
			return this.Literal(_literal_0, true);
		}

		protected Boolean PhoneNumber_impl_5()
		{
			String _nodeName_0 = "ThreeDigitCode";
			return this.CapturingGroup(new IsMatchPredicate(this.PhoneNumber_impl_6), _nodeName_0, false, false);
		}

		protected Boolean PhoneNumber_impl_6()
		{
			return this.LimitingRepetition(new IsMatchPredicate(this.PhoneNumber_impl_7), 3, 3, "");
		}

		protected Boolean PhoneNumber_impl_7()
		{
			String _classExpression_0 = "[0-9]";
			return this.CharacterClass(_classExpression_0, 5);
		}
	}
}
