using System;
using System.Text;
using RobustHaven.Text.Npeg;

namespace RobustHaven.Text.Npeg.Tests.Parsers
{
	public class OneOrMoreInfiniteLoopTest : NpegParser
	{
		public OneOrMoreInfiniteLoopTest(InputIterator iterator): base(iterator){}

		public override Boolean IsMatch()
		{
			return OneOrMoreInfiniteLoopTest_impl_0();
		}


		protected Boolean OneOrMoreInfiniteLoopTest_impl_0()
		{
			String _nodeName_0 = "Expression";
			return this.CapturingGroup(new IsMatchPredicate(this.OneOrMoreInfiniteLoopTest_impl_1), _nodeName_0, false, false);
		}

		protected Boolean OneOrMoreInfiniteLoopTest_impl_1()
		{
			return this.OneOrMore(new IsMatchPredicate(this.OneOrMoreInfiniteLoopTest_impl_2), "");
		}

		protected Boolean OneOrMoreInfiniteLoopTest_impl_2()
		{
			return this.ZeroOrMore(new IsMatchPredicate(this.OneOrMoreInfiniteLoopTest_impl_3), "");
		}

		protected Boolean OneOrMoreInfiniteLoopTest_impl_3()
		{
			return this.AnyCharacter();
		}
	}
}
