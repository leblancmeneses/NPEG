using System;

namespace RobustHaven.Text.Npeg.Tests.Parsers
{
	public class OneOrMoreInfiniteLoopTest : NpegParser
	{
		public OneOrMoreInfiniteLoopTest(InputIterator iterator) : base(iterator)
		{
		}

		public override Boolean IsMatch()
		{
			return OneOrMoreInfiniteLoopTest_impl_0();
		}


		protected Boolean OneOrMoreInfiniteLoopTest_impl_0()
		{
			String _nodeName_0 = "Expression";
			return CapturingGroup(OneOrMoreInfiniteLoopTest_impl_1, _nodeName_0, false, false);
		}

		protected Boolean OneOrMoreInfiniteLoopTest_impl_1()
		{
			return OneOrMore(OneOrMoreInfiniteLoopTest_impl_2, "");
		}

		protected Boolean OneOrMoreInfiniteLoopTest_impl_2()
		{
			return ZeroOrMore(OneOrMoreInfiniteLoopTest_impl_3, "");
		}

		protected Boolean OneOrMoreInfiniteLoopTest_impl_3()
		{
			return AnyCharacter();
		}
	}
}