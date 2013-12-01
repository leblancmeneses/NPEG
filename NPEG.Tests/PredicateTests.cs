using System.Text;
using NPEG.NonTerminals;
using NPEG.Terminals;
using NUnit.Framework;

namespace NPEG.Tests
{
	[TestFixture]
	public class PredicateTests
	{
		public TestContext TestContext { get; set; }

		[Test]
		public void NonTerminal_Predicate_And()
		{
			// predicates should not adjust the 
			// iterator once the expression is evaluated.
			AExpression Digit = new CharacterClass {ClassExpression = "[0-9]"};

			// regex expression: \d+
			var input = Encoding.UTF8.GetBytes("01234567890123456789");
			var iterator = new ByteInputIterator(input);
			AExpression andPredicate = new OneOrMore(Digit).And();
			var visitor = new NpegParserVisitor(iterator);
			andPredicate.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			Assert.IsTrue(iterator.Index == 0);
		}


		[Test]
		public void NonTerminal_Predicate_Or()
		{
			// predicates should not adjust the 
			// iterator once the expression is evaluated.
			AExpression Digit = new CharacterClass {ClassExpression = "[0-9]"};

			// equivalent to: regex '^' '$'
			// regex expression: ^\d+$
			var bytes = Encoding.UTF8.GetBytes("0123456abcdefg");
			var iterator = new ByteInputIterator(bytes);
			AExpression notPredicate = new OneOrMore(Digit).And().Sequence(new NotPredicate(new AnyCharacter()));
			var visitor = new NpegParserVisitor(iterator);
			notPredicate.Accept(visitor);
			Assert.IsFalse(visitor.IsMatch); // should fail
			Assert.IsTrue(iterator.Index == 0);
		}
	}
}