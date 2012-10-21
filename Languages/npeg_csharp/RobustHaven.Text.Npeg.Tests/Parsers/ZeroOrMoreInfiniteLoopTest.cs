using System;

namespace RobustHaven.Text.Npeg.Tests.Parsers
{
	public class ZeroOrMoreInfiniteLoopTest : NpegParser
	{
		public ZeroOrMoreInfiniteLoopTest(InputIterator iterator) : base(iterator)
		{
		}

		public override Boolean IsMatch()
		{
			return ZeroOrMoreInfiniteLoopTest_impl_0();
		}


		protected Boolean ZeroOrMoreInfiniteLoopTest_impl_0()
		{
			String _nodeName_0 = "Expression";
			return CapturingGroup(ZeroOrMoreInfiniteLoopTest_impl_1, _nodeName_0, false, false);
		}

		protected Boolean ZeroOrMoreInfiniteLoopTest_impl_1()
		{
			return ZeroOrMore(ZeroOrMoreInfiniteLoopTest_impl_2, "");
		}

		protected Boolean ZeroOrMoreInfiniteLoopTest_impl_2()
		{
			return ZeroOrMore(ZeroOrMoreInfiniteLoopTest_impl_3, "");
		}

		protected Boolean ZeroOrMoreInfiniteLoopTest_impl_3()
		{
			return AnyCharacter();
		}
	}
}