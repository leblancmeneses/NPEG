using System;

namespace RobustHaven.Text.Npeg.Tests.Parsers
{
	public class LimitingRepetitionInfiniteLoopTest : NpegParser
	{
		public LimitingRepetitionInfiniteLoopTest(InputIterator iterator) : base(iterator)
		{
		}

		public override Boolean IsMatch()
		{
			return LimitingRepetitionInfiniteLoopTest_impl_0();
		}


		protected Boolean LimitingRepetitionInfiniteLoopTest_impl_0()
		{
			String _nodeName_0 = "Expression";
			return CapturingGroup(LimitingRepetitionInfiniteLoopTest_impl_1, _nodeName_0, false, false);
		}

		protected Boolean LimitingRepetitionInfiniteLoopTest_impl_1()
		{
			return LimitingRepetition(LimitingRepetitionInfiniteLoopTest_impl_2, 0, null, "");
		}

		protected Boolean LimitingRepetitionInfiniteLoopTest_impl_2()
		{
			return ZeroOrMore(LimitingRepetitionInfiniteLoopTest_impl_3, "");
		}

		protected Boolean LimitingRepetitionInfiniteLoopTest_impl_3()
		{
			return AnyCharacter();
		}
	}
}