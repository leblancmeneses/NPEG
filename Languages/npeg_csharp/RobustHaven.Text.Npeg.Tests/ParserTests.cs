using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RobustHaven.Text.Npeg;
using NUnit.Framework;

namespace RobustHaven.Text.Npeg.Tests
{
    [TestFixture]
    public class ParserTests
    {
        [Test]
        public void SimpleXmlTest()
        {
            Parsers.SimpleXml xmlparser = new Parsers.SimpleXml(
                new StringInputIterator(
                    Encoding.ASCII.GetBytes(
                        "<h1>hello</h1><h2>hello</h2>"
                    )
                )
            );
            Assert.IsTrue(xmlparser.IsMatch());

        }

        [Test]
        public void MathematicalFormulaTest()
        {
            Parsers.MathematicalFormula parser = new Parsers.MathematicalFormula(
                new StringInputIterator(
                    Encoding.ASCII.GetBytes(
                        "((((12/3)+5-2*(81/9))+1))"
                    )
                )
            );
            Assert.IsTrue(parser.IsMatch());
        }

        [Test]
        public void PhoneNumberTest()
        {
            Parsers.PhoneNumber parser = new Parsers.PhoneNumber(
                new StringInputIterator(
                    Encoding.ASCII.GetBytes(
                        "123-456-7890"
                    )
                )
            );
            Assert.IsTrue(parser.IsMatch());
        }


        [Test]
        public void NpegNodeTest()
        {
            Parsers.NpegNode parser = new Parsers.NpegNode(
                new StringInputIterator(
                    Encoding.ASCII.GetBytes(
                        "NpEg"
                    )
                )
            );
            Assert.IsTrue(parser.IsMatch());
        }
    }
}
