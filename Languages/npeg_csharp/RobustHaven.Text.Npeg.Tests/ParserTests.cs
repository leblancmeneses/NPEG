using System.Text;
using NUnit.Framework;
using RobustHaven.Text.Npeg.Tests.Parsers;

namespace RobustHaven.Text.Npeg.Tests
{
	[TestFixture]
	public class ParserTests
	{
		[Test]
		public void MathematicalFormulaTest()
		{
			var parser = new MathematicalFormula(
				new StringInputIterator(
					Encoding.ASCII.GetBytes(
						"((((12/3)+5-2*(81/9))+1))"
						)
					)
				);
			Assert.IsTrue(parser.IsMatch());
		}


		[Test]
		public void NpegNodeTest()
		{
			var parser = new NpegNode(
				new StringInputIterator(
					Encoding.ASCII.GetBytes(
						"NpEg"
						)
					)
				);
			Assert.IsTrue(parser.IsMatch());
		}

		[Test]
		public void PhoneNumberTest()
		{
			var parser = new PhoneNumber(
				new StringInputIterator(
					Encoding.ASCII.GetBytes(
						"123-456-7890"
						)
					)
				);
			Assert.IsTrue(parser.IsMatch());
		}

		[Test]
		public void SimpleXmlTest()
		{
			var xmlparser = new SimpleXml(
				new StringInputIterator(
					Encoding.ASCII.GetBytes(
						"<h1>hello</h1><h2>hello</h2>"
						)
					)
				);
			Assert.IsTrue(xmlparser.IsMatch());
		}
	}
}