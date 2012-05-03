using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using NPEG;
using NPEG.Terminals;
using NPEG.NonTerminals;

namespace NPEG.Tests
{
    [TestClass]
    public class TerminalNodes
    {
        public TerminalNodes()
        {
        }

        public TestContext TestContext
        {
            get;
            set;
        }

        [TestMethod]
        public void Terminal_Any()
        {
            var input = new StringInputIterator("ijk");
            AExpression any = new Sequence(new AnyCharacter(), new AnyCharacter());
            var visitor = new NpegParserVisitor(input);
            any.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            Assert.IsTrue(input.Index == 2, "Expected two characters to be consumed and Iterator updated by 2.  0, 1 .. points to 2");

            input = new StringInputIterator("ij");
            visitor = new NpegParserVisitor(input);
            any.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            Assert.IsTrue(input.Index == 2, "Expected two characters to be consumed and Iterator updated by 2.  0, 1 .. points to 2");

            input = new StringInputIterator("");
            any = new AnyCharacter();
            visitor = new NpegParserVisitor(input);
            any.Accept(visitor);
            Assert.IsFalse(visitor.IsMatch);
            Assert.IsTrue(input.Index == 0, "Expected no characters to be consumed and index stay at zero.");

            var number = new Sequence(
                new OneOrMore(new CharacterClass() { ClassExpression = "[0-9]" }), 
                    new NotPredicate(
                        new AnyCharacter()
                    )
                );
            input = new StringInputIterator("012345.");
            visitor = new NpegParserVisitor(input);
            number.Accept(visitor);
            Assert.IsFalse(visitor.IsMatch);
        }



        [TestMethod]
        public void Terminal_CharacterClass()
        {
            AExpression Digit = new CharacterClass() { ClassExpression = "[0-9]" };

            NpegParserVisitor visitor = new NpegParserVisitor(new StringInputIterator("0"));
            Digit.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);

            visitor = new NpegParserVisitor(new StringInputIterator("0123456789"));
            new OneOrMore(Digit).Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
        }

        [TestMethod]
        public void Terminal_Literal()
        {
            var Mixed = new Literal() { MatchText = "Hello World" };

            NpegParserVisitor visitor = new NpegParserVisitor(new StringInputIterator("hello world"));
            Mixed.Accept(visitor);
            Assert.IsFalse(visitor.IsMatch);

            // Not case sensitve
            Mixed.IsCaseSensitive = false;
            visitor = new NpegParserVisitor(new StringInputIterator("hello world"));
            Mixed.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
        }


        [TestMethod]
        public void Terminal_CodePoint_Hexadecimal()
        {
            Assert.IsTrue((Byte)'a' == (Byte)97);
            Assert.IsTrue((Byte)'a' == (Byte)0x61);

            var visitor = new NpegParserVisitor(new StringInputIterator("a"));
            var Hexadecimal = new CapturingGroup("Hexadecimal",
                new CodePoint() { Match = "#x61" }
            );
            Hexadecimal.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            var node = visitor.AST;
            Assert.IsTrue(node.Token.Name == "Hexadecimal");
            Assert.IsTrue(node.Token.Value == "a");


            // Byte boundary tests
            visitor = new NpegParserVisitor(new StringInputIterator("\na"));
            Hexadecimal = new CapturingGroup("Hexadecimal",
                new CodePoint() { Match = "#xA61" }
            );
            Hexadecimal.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch, "During incomplete byte boundaries 0 is expected to prefix input;  This would shift input to the right by 4 bits.  In this case it complete codepoint should be 0A = \n and letter a.");
            node = visitor.AST;
            Assert.IsTrue(node.Token.Name == "Hexadecimal");
            Assert.IsTrue(node.Token.Value == "\na");



            visitor = new NpegParserVisitor(new StringInputIterator("\0a"));
            Hexadecimal = new CapturingGroup("Hexadecimal",
                new CodePoint() { Match = "#x061" }
            );
            Hexadecimal.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch, "During incomplete byte boundaries 0 is expected to prefix input;  This would shift input to the right by 4 bits.  In this case it complete codepoint should be 00 = \0 and letter a.");
            node = visitor.AST;
            Assert.IsTrue(node.Token.Name == "Hexadecimal");
            Assert.IsTrue(node.Token.Value == "\0a");


            // Don't care tests
            visitor = new NpegParserVisitor(new StringInputIterator(Encoding.ASCII.GetString(new byte[] { 0x11, 0x01, 0x71, 0x03, 0x00 })));
            Hexadecimal = new CapturingGroup("Hexadecimal",
                new OneOrMore(new CodePoint() { Match = "#xX1" }) // #bXXXX0001
            );
            Hexadecimal.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            node = visitor.AST;
            Assert.IsTrue(node.Token.Name == "Hexadecimal");
            Assert.IsTrue(node.Token.Value == Encoding.ASCII.GetString(new byte[] { 0x11, 0x01, 0x71 }));


            visitor = new NpegParserVisitor(new StringInputIterator(
                    Encoding.ASCII.GetString(new byte[] { 0x10 })
                ));
            Hexadecimal = new CapturingGroup("Hexadecimal",
                new CodePoint() { Match = "#xX1" }
            );
            Hexadecimal.Accept(visitor);
            Assert.IsFalse(visitor.IsMatch);


            // cannot consume character test
            visitor = new NpegParserVisitor(new StringInputIterator(""));
            Hexadecimal = new CapturingGroup("Hexadecimal",
                new CodePoint() { Match = "#xX1" }
            );
            Hexadecimal.Accept(visitor);
            Assert.IsFalse(visitor.IsMatch);
        }





        [TestMethod]
        public void Terminal_CodePoint_Binary()
        {
            Assert.IsTrue((Byte)'a' == (Byte)97);
            Assert.IsTrue((Byte)'a' == (Byte)0x61);

            var visitor = new NpegParserVisitor(new StringInputIterator("a"));
            var binary = new CapturingGroup("Binary",
                new CodePoint() { Match = "#b1100001" }
            );
            binary.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            var ast = visitor.AST;
            Assert.IsTrue(ast.Token.Name == "Binary");
            Assert.IsTrue(ast.Token.Value == "a");


            visitor = new NpegParserVisitor(new StringInputIterator("aa"));
            binary = new CapturingGroup("Binary",
                new CodePoint() { Match = "#b0110000101100001" }
            );
            binary.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            ast = visitor.AST;
            Assert.IsTrue(ast.Token.Name == "Binary");
            Assert.IsTrue(ast.Token.Value == "aa");



            // Byte boundary tests
            visitor = new NpegParserVisitor(new StringInputIterator("\0a"));
            binary = new CapturingGroup("Binary",
                new CodePoint() { Match = "#b00001100001" }
            );
            binary.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch, "During incomplete byte boundaries 0 is expected to prefix input;  This would shift input to the right by 4 bits.  In this case it complete codepoint should be null and letter a.");
            ast = visitor.AST;
            Assert.IsTrue(ast.Token.Name == "Binary");
            Assert.IsTrue(ast.Token.Value == "\0a");


            visitor = new NpegParserVisitor(new StringInputIterator("\0a"));
            binary = new CapturingGroup("Binary",
                new Sequence(new CodePoint() { Match = "#b000" }, new CodePoint(){ Match="#b01100001"})
            );
            binary.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch, "During incomplete byte boundaries 0 is expected to prefix input;  This would shift input to the right by 4 bits.  In this case it complete codepoint should be null and letter a.");
            ast = visitor.AST;
            Assert.IsTrue(ast.Token.Name == "Binary");
            Assert.IsTrue(ast.Token.Value == "\0a");



            // Don't care tests
            visitor = new NpegParserVisitor(new StringInputIterator(Encoding.ASCII.GetString(new byte[] { 0x11, 0x01, 0x71, 0x03, 0x00 })));
            binary = new CapturingGroup("Binary",
                new OneOrMore(new CodePoint() { Match = "#bXXXX0001" }) // #bXXXX0001
            );
            binary.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            ast = visitor.AST;
            Assert.IsTrue(ast.Token.Name == "Binary");
            Assert.IsTrue(ast.Token.Value == Encoding.ASCII.GetString(new byte[] { 0x11, 0x01, 0x71 }));


            visitor = new NpegParserVisitor(new StringInputIterator(
                    Encoding.ASCII.GetString(new byte[] { 0x10 })
                ));
            binary = new CapturingGroup("Binary",
                new CodePoint() { Match = "#bXXXX0001" }
            );
            binary.Accept(visitor);
            Assert.IsFalse(visitor.IsMatch);


            // cannot consume character test
            visitor = new NpegParserVisitor(new StringInputIterator(""));
            binary = new CapturingGroup("Binary",
                new CodePoint() { Match = "#bXXXX0001" }
            );
            binary.Accept(visitor);
            Assert.IsFalse(visitor.IsMatch);
        }


        [TestMethod]
        public void Terminal_CodePoint_Decimal()
        {
            var visitor = new NpegParserVisitor(new StringInputIterator("&"));
            var codepoint = new CapturingGroup("CodePoint",
                new CodePoint() { Match = "#38" }
            );
            codepoint.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            var ast = visitor.AST;
            Assert.IsTrue(ast.Token.Name == "CodePoint");
            Assert.IsTrue(ast.Token.Value == "&");
        }





        [TestMethod]
        public void Terminal_LimitingRepetition()
        {
            // min
            // min max
            // max

            AExpression Digits = new CharacterClass() { ClassExpression = "[0-9]" };

            #region nonterminals
            var MinTrue0 = new CapturingGroup("MinTrue",
                new LimitingRepetition(Digits) { Min = 0 }

            );
            var MinFalse = new CapturingGroup("MinFalse",
                new LimitingRepetition(Digits) { Min = 44 }
            );



            var MinTrue5 = new CapturingGroup("MinTrue",
                new LimitingRepetition(Digits) { Min = 5 }
            );
            var MaxTrue = new CapturingGroup("MaxTrue",
                new LimitingRepetition(Digits) { Max = 5 }
            );
            var MinMax = new CapturingGroup("MinMax",
                new LimitingRepetition(Digits) { Min = 5, Max = 6 }
            );


            var ExceptionNoMinMax = new CapturingGroup("ExceptionNoMinMax",
                new LimitingRepetition(Digits) { }
            );
            var ExceptionMaxLessThanMin = new CapturingGroup("ExceptionMaxLessThanMin",
                new LimitingRepetition(Digits) { Min = 5, Max = 0 }
            );
            #endregion

            String input = "1234567890";
            StringInputIterator iterator = new StringInputIterator(input);
            NpegParserVisitor visitor = new NpegParserVisitor(iterator);

            MinTrue0.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            AstNode node = visitor.AST;

            iterator.Index = 0;
            visitor = new NpegParserVisitor(iterator);
            MinFalse.Accept(visitor);
            Assert.IsFalse(visitor.IsMatch);


            iterator.Index = 0;
            visitor = new NpegParserVisitor(iterator);
            MinTrue5.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            node = visitor.AST;
            Assert.IsTrue(node.Token.Value == input);


            iterator.Index = 0;
            visitor = new NpegParserVisitor(iterator);
            MaxTrue.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            node = visitor.AST;


            iterator.Index = 0;
            visitor = new NpegParserVisitor(iterator);
            MinMax.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            node = visitor.AST;


            Int32 exceptionCount = 0;
            try
            {
                iterator.Index = 0;
                visitor = new NpegParserVisitor(iterator);
                ExceptionNoMinMax.Accept(visitor);
                Assert.IsTrue(visitor.IsMatch);
                node = visitor.AST;
            }
            catch (ArgumentException) { exceptionCount++; }

            try
            {
                iterator.Index = 0;
                visitor = new NpegParserVisitor(iterator);
                ExceptionMaxLessThanMin.Accept(visitor);
                Assert.IsTrue(visitor.IsMatch);
                node = visitor.AST;
            }
            catch (ArgumentException) { exceptionCount++; }


            Assert.IsTrue(exceptionCount == 2);
        }




        [TestMethod]
        public void Terminal_DynamicBackReference()
        {
            #region Composite
            AExpression TAG = new CapturingGroup("TAG", 
                          new OneOrMore(
                              new CharacterClass() { ClassExpression = "[a-zA-Z0-9]" }
                          )
                      );

            AExpression StartTag = new CapturingGroup("START_TAG", 
                                new Sequence(
                                    new Literal() { MatchText = "<" }, TAG)
                                    .Sequence(
                                    new Literal() { MatchText = ">" }
                                )
                           );

            AExpression EndTag = new CapturingGroup("END_TAG", 
                                new Sequence(
                                    new Literal() { MatchText = "</" },
                                    new DynamicBackReference()
                                    {
                                        BackReferenceName = "TAG",
                                        IsCaseSensitive = true
                                    }
                                )
                                .Sequence(
                                    new Literal() { MatchText = ">" }
                                )
                            );


            AExpression Body = new CapturingGroup("Body", new Sequence(new NotPredicate(EndTag), new AnyCharacter()).Star());

            AExpression Expression = new CapturingGroup("Expression", new Sequence(StartTag, Body).Sequence(EndTag).Plus());
            #endregion

            String input = "<h1>hello</h1><h2>hello</h2>";
            StringInputIterator iterator = new StringInputIterator(input);
            NpegParserVisitor visitor = new NpegParserVisitor(iterator);

            Expression.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            var ast = visitor.AST;
#warning write tree
        }




        [TestMethod]
        public void Terminal_DynamicBackReference_Recursive()
        {
            String input =
                @"
                    <test>
                        test data start
                        <test1>
                            test1 data start
                            <test2>
                                text2 data start
                                text2 data end
                            </test2>
                            test1 data end
                        </test1>
                        test data end
                    </test>
                ";

            var TAG = new CapturingGroup("TAG",
                  new OneOrMore(
                      new CharacterClass() { ClassExpression = "[a-zA-Z0-9]" }
                  )
            );

            var StartTag = new CapturingGroup("START_TAG",
                                new Sequence(
                                    new Literal() { MatchText = "<" }, TAG)
                                    .Sequence(
                                    new Literal() { MatchText = ">" }
                                )
                           );

            var EndTag = new CapturingGroup("END_TAG",
                                new Sequence(
                                    new Literal() { MatchText = "</" },
                                    new DynamicBackReference()
                                    {
                                        BackReferenceName = "TAG",
                                        IsCaseSensitive = true
                                    }
                                )
                                .Sequence(
                                    new Literal() { MatchText = ">" }
                                )
                            );


            var Body = new CapturingGroup("Body", 
                    new PrioritizedChoice(
                        new RecursionCall("MATCHXML"),
                        new Sequence(new NotPredicate(EndTag), new AnyCharacter())
                    ).Star()
                );

            var Expression = new CapturingGroup("Expression",
                                new RecursionCreate("MATCHXML", 
                                    new Sequence(StartTag, Body)
                                        .Sequence(EndTag)
                                        .Plus()
                                )
                             );


            StringInputIterator iterator = new StringInputIterator(input.Trim());
            NpegParserVisitor visitor = new NpegParserVisitor(iterator);

            Expression.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            AstNode node = visitor.AST;
        }
    }
}
