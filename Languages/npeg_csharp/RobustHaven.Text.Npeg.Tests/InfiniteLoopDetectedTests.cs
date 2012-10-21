using System.Text;
using NUnit.Framework;
using RobustHaven.Text.Npeg.Tests.Parsers;

namespace RobustHaven.Text.Npeg.Tests
{
	[TestFixture]
	public class InfiniteLoopDetectedTests
	{
		[Test]
		[Timeout(2000)]
		[ExpectedException(typeof (InfiniteLoopDetectedException))]
		public void LimitingRepetition()
		{
			var parser = new LimitingRepetitionInfiniteLoopTest(
				new StringInputIterator(
					Encoding.ASCII.GetBytes(""))
				);
			parser.IsMatch();
		}

		[Test]
		[Timeout(2000)]
		[ExpectedException(typeof (InfiniteLoopDetectedException))]
		public void OneOrMore()
		{
			var parser = new OneOrMoreInfiniteLoopTest(
				new StringInputIterator(
					Encoding.ASCII.GetBytes(""))
				);
			parser.IsMatch();
		}

		[Test]
		[Timeout(2000)]
		[ExpectedException(typeof (InfiniteLoopDetectedException))]
		public void ZeroOrMore()
		{
			var parser = new ZeroOrMoreInfiniteLoopTest(
				new StringInputIterator(
					Encoding.ASCII.GetBytes(""))
				);
			parser.IsMatch();
		}
	}
}