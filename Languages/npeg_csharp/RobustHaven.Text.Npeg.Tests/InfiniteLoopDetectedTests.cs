using System;
using System.Text;

using NUnit.Framework;

namespace RobustHaven.Text.Npeg.Tests
{
    [TestFixture]
    public class InfiniteLoopDetectedTests
    {
        [Test]
        [TimeoutAttribute(2000)]
        [ExpectedException(typeof(InfiniteLoopDetectedException))]
        public void ZeroOrMore()
        {
            Parsers.ZeroOrMoreInfiniteLoopTest parser = new Parsers.ZeroOrMoreInfiniteLoopTest(
                new StringInputIterator( 
                    Encoding.ASCII.GetBytes(""))
                );
            parser.IsMatch();
        }


        [Test]
        [TimeoutAttribute(2000)]
        [ExpectedException(typeof(InfiniteLoopDetectedException))]
        public void OneOrMore()
        {
            Parsers.OneOrMoreInfiniteLoopTest parser = new Parsers.OneOrMoreInfiniteLoopTest(
                new StringInputIterator(
                    Encoding.ASCII.GetBytes(""))
                );
            parser.IsMatch();
        }


        [Test]
        [TimeoutAttribute(2000)]
        [ExpectedException(typeof(InfiniteLoopDetectedException))]
        public void LimitingRepetition()
        {
            Parsers.LimitingRepetitionInfiniteLoopTest parser = new Parsers.LimitingRepetitionInfiniteLoopTest(
                new StringInputIterator(
                    Encoding.ASCII.GetBytes(""))
                );
            parser.IsMatch();
        }
    }
}
