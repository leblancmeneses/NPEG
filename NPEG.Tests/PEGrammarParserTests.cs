using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NPEG.Tests
{
    [TestClass]
    public class PEGrammarParserTests
    {
        public PEGrammarParserTests()
        {
        }

        public TestContext TestContext
        {
            get;
            set;
        }


        [TestMethod]
        public void PEGrammarParser_Space()
        {
            var rule = this.OneOrMore(this.GetRule("mSpace"));
            var root = this.WrapInCapturedGroup("Test", this.RequireEndOfInput(rule));

            var input = new StringInputIterator(" \t         \t\t           \t");
            var visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
        }
        

        [TestMethod]
        public void PEGrammarParser_NewLine()
        {
            var rule = this.OneOrMore(this.GetRule("mNewLine"));
            var root = this.WrapInCapturedGroup("Test", this.RequireEndOfInput(rule));

            var input = new StringInputIterator("\n\n\r\n\r\r");  // notice only matches newlines of linux/win/mac/ 
            var visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
        }


        [TestMethod]
        public void PEGrammarParser_Comment()
        {
            var rule = this.OneOrMore(this.GetRule("mComment"));
            var root = this.WrapInCapturedGroup("Test", this.RequireEndOfInput(rule));

            var input = new StringInputIterator(@"//this is a single line comment.");
            var visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);


            input = new StringInputIterator(@"/*this is a multiline comment.*/");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);


            input = new StringInputIterator(@"/*
                        this 
                        is 
                        a   multiline 
                        comment.
                    */");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
        }


        [TestMethod]
        public void PEGrammarParser_WhiteSpace()
        {
            var rule = this.OneOrMore(this.GetRule("mWhiteSpace"));
            var root = this.WrapInCapturedGroup("Test", this.RequireEndOfInput(rule));

            // In the grammar interpreter whitespace is a composite of: 
            // newline, comment, space rules
            var input = new StringInputIterator(@"
                //this is a single line comment.
                //this is a single line comment.
                /*
                        this 
                        is 
                        a   multiline 
                        comment.
                    */
                //this is a single line comment.
                //this is a single line comment.
                /*
                        this 
                        is 
                        a   multiline 
                        comment.
                    */
            ");
            var visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
        }


        [TestMethod]
        public void PEGrammarParser_Hex()
        {
            var rule = this.GetRule("mHex");
            var root = this.WrapInCapturedGroup("Test", this.RequireEndOfInput(rule));

            var input = new StringInputIterator("0x012345679aaabbbcccdddeeeff");
            var visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);

            input = new StringInputIterator("x012345679aaabbbcccdddeeeff");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);


            // expected to fail
            input = new StringInputIterator("x012345679ggghhhiiijjjkkklllmmmnnn");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsFalse(visitor.IsMatch);
        }


        [TestMethod]
        public void PEGrammarParser_Binary()
        {
            var rule = this.GetRule("mBinary");
            var root = this.WrapInCapturedGroup("Test", this.RequireEndOfInput(rule));

            var input = new StringInputIterator("0b01000101010101010");
            var visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);

            input = new StringInputIterator("b01000101010101010");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);

            // expected to fail
            input = new StringInputIterator("0b0103234234");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsFalse(visitor.IsMatch);
        }


        [TestMethod]
        public void PEGrammarParser_Label()
        {
            var rule = this.GetRule("mLabel");
            var root = this.WrapInCapturedGroup("Test", this.RequireEndOfInput(rule));

            var input = new StringInputIterator("variaBle023_name");
            var visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);

            AstNode node = visitor.AST;
            Assert.IsTrue(node.Children[0].Token.Name == "Label");
            Assert.IsTrue(node.Children[0].Token.Value == "variaBle023_name");


            input = new StringInputIterator("_variaBle023_name");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);

            input = new StringInputIterator("AAvariaBle023_name");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);

            // expected to fail
            input = new StringInputIterator("2invalidvarname");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsFalse(visitor.IsMatch);
        }



        [TestMethod]
        public void PEGrammarParser_AnyCharacter()
        {
            var rule = this.GetRule("mAnyCharacter");
            var root = this.WrapInCapturedGroup("Test", this.RequireEndOfInput(rule));

            var input = new StringInputIterator(".");
            var visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);

            AstNode node = visitor.AST;
            Assert.IsTrue(node.Token.Value == ".");
        }

        [TestMethod]
        public void PEGrammarParser_CharacterClass()
        {
            var rule = this.GetRule("mCharacterClass");
            var root = this.WrapInCapturedGroup("Test", this.RequireEndOfInput(rule));

            var input = new StringInputIterator("[a-zA-Z0-9_-]");
            var visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);

            input = new StringInputIterator("[^a-zA-Z0-9_-]");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
        }


        [TestMethod]
        public void PEGrammarParser_CodePoint()
        {
            var rule = this.GetRule("mCodePoint");
            var root = this.WrapInCapturedGroup("Test", this.RequireEndOfInput(rule));

            var input = new StringInputIterator(@"#0b01010101");
            var visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);

            var node = visitor.AST;
            Assert.IsTrue(node.Children[0].Token.Name == "CodePoint");
            Assert.IsTrue(node.Children[0].Token.Value == "#0b01010101");

            input = new StringInputIterator(@"#2563");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);

            node = visitor.AST;
            Assert.IsTrue(node.Token.Name == "Test");
            Assert.IsTrue(node.Children[0].Token.Name == "CodePoint");
            Assert.IsTrue(node.Children[0].Token.Value == "#2563");


            input = new StringInputIterator(@"#0xffff");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);

            node = visitor.AST;
            Assert.IsTrue(node.Children[0].Token.Name == "CodePoint");
            Assert.IsTrue(node.Children[0].Token.Value == "#0xffff");
        }


        [TestMethod]
        public void PEGrammarParser_DynamicBackReference()
        {
            var rule = this.GetRule("mDynamicBackReference");
            var root = this.WrapInCapturedGroup("Test", this.RequireEndOfInput(rule));

            var input = new StringInputIterator(@"\k< CapturedLabelVariableName >");
            var visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            var node = visitor.AST;
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Label");
            Assert.IsTrue(node.Children[0].Children[0].Token.Value == "CapturedLabelVariableName");


            
            input = new StringInputIterator(@"\k<CapturedLabelVariableName>");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            node = visitor.AST;
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Label");
            Assert.IsTrue(node.Children[0].Children[0].Token.Value == "CapturedLabelVariableName");

            

            input = new StringInputIterator(@"\k< CapturedLabelVariableName[\i] >");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            node = visitor.AST;
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Label");
            Assert.IsTrue(node.Children[0].Children[0].Token.Value == "CapturedLabelVariableName");
            Assert.IsTrue(node.Children[0].Children[1].Token.Name == "CaseSensitive");
            Assert.IsTrue(node.Children[0].Children[1].Token.Value == @"\i");


            
            input = new StringInputIterator(@"\k< CapturedLabelVariableName   [\i] >");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            node = visitor.AST;
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Label");
            Assert.IsTrue(node.Children[0].Children[0].Token.Value == "CapturedLabelVariableName");
            Assert.IsTrue(node.Children[0].Children[1].Token.Name == "CaseSensitive");
            Assert.IsTrue(node.Children[0].Children[1].Token.Value == @"\i");
        }


        [TestMethod]
        public void PEGrammarParser_FATAL()
        {
            //It's up to the interpreter to expand: escaped \' and \" to the correct form ' and "

            var rule = this.GetRule("mFATAL");
            var root = this.WrapInCapturedGroup("Test", this.RequireEndOfInput(rule));

            var input = new StringInputIterator(@"FaTal< fatal message without single or double quotes >");
            var visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            var node = visitor.AST;
            Assert.IsTrue(node.Children[0].Token.Name == "Fatal");
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Message");
            Assert.IsTrue(node.Children[0].Children[0].Token.Value == "fatal message without single or double quotes");



            input = new StringInputIterator(@"FATAL< 'Fatal with single quotes' >");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            node = visitor.AST;
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Message");
            Assert.IsTrue(node.Children[0].Children[0].Token.Value == "Fatal with single quotes");



            input = new StringInputIterator(@"FATAL<'Fatal with single quotes'>");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            node = visitor.AST;
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Message");
            Assert.IsTrue(node.Children[0].Children[0].Token.Value == "Fatal with single quotes");


            input = new StringInputIterator(@"FATAL<'Fatal\'s escaped single quotes'>");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            node = visitor.AST;
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Message");
            Assert.IsTrue(node.Children[0].Children[0].Token.Value == @"Fatal\'s escaped single quotes");


            input = new StringInputIterator(@"FATAL< ""Fatal with double quotes"" >");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            node = visitor.AST;
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Message");
            Assert.IsTrue(node.Children[0].Children[0].Token.Value == "Fatal with double quotes");



            input = new StringInputIterator(@"FATAL<""Fatal with double quotes"">");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            node = visitor.AST;
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Message");
            Assert.IsTrue(node.Children[0].Children[0].Token.Value == "Fatal with double quotes");


            input = new StringInputIterator(@"FATAL<""Fatal message quoteing \""some other message\"" using escapes"">");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            node = visitor.AST;
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Message");
            Assert.IsTrue(node.Children[0].Children[0].Token.Value == @"Fatal message quoteing \""some other message\"" using escapes");
        }


        
        [TestMethod]
        public void PEGrammarParser_WARN()
        {
            //It's up to the interpreter to expand: escaped \' and \" to the correct form ' and "

            var rule = this.GetRule("mWARN");
            var root = this.WrapInCapturedGroup("Test", this.RequireEndOfInput(rule));

            var input = new StringInputIterator(@"WarN< warning message without single or double quotes >");
            var visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            var node = visitor.AST;
            Assert.IsTrue(node.Children[0].Token.Name == "Warn");
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Message");
            Assert.IsTrue(node.Children[0].Children[0].Token.Value == "warning message without single or double quotes");



            input = new StringInputIterator(@"WARN< 'warning with single quotes' >");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            node = visitor.AST;
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Message");
            Assert.IsTrue(node.Children[0].Children[0].Token.Value == "warning with single quotes");



            input = new StringInputIterator(@"WARN<'warning with single quotes'>");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            node = visitor.AST;
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Message");
            Assert.IsTrue(node.Children[0].Children[0].Token.Value == "warning with single quotes");


            input = new StringInputIterator(@"WARN<'warning\'s escaped single quotes'>");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            node = visitor.AST;
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Message");
            Assert.IsTrue(node.Children[0].Children[0].Token.Value == @"warning\'s escaped single quotes");


            input = new StringInputIterator(@"WARN< ""warning with double quotes"" >");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            node = visitor.AST;
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Message");
            Assert.IsTrue(node.Children[0].Children[0].Token.Value == "warning with double quotes");



            input = new StringInputIterator(@"WARN<""warning with double quotes"">");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            node = visitor.AST;
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Message");
            Assert.IsTrue(node.Children[0].Children[0].Token.Value == "warning with double quotes");


            input = new StringInputIterator(@"WARN<""warning message quoteing \""some other message\"" using escapes"">");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            node = visitor.AST;
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Message");
            Assert.IsTrue(node.Children[0].Children[0].Token.Value == @"warning message quoteing \""some other message\"" using escapes");
        }



        [TestMethod]
        public void PEGrammarParser_Literal()
        {
            var rule = this.GetRule("mLiteral");
            var root = this.WrapInCapturedGroup("Test", this.RequireEndOfInput(rule));

            var input = new StringInputIterator(@"'this is some captured text'");
            var visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            var node = visitor.AST;
            Assert.IsTrue(node.Children[0].Token.Name == "Literal");
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "MatchText");
            Assert.IsTrue(node.Children[0].Children[0].Token.Value == "this is some captured text");


            input = new StringInputIterator(@"""this is double quoted captured text""");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            node = visitor.AST;
            Assert.IsTrue(node.Children[0].Token.Name == "Literal");
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "MatchText");
            Assert.IsTrue(node.Children[0].Children[0].Token.Value == "this is double quoted captured text");
        }


        [TestMethod]
        public void PEGrammarParser_CapturingGroup()
        {
            var rule = this.GetRule("mExpressionRoot");
            var root = this.WrapInCapturedGroup("Test", this.RequireEndOfInput(rule));

            var input = new StringInputIterator(@"(?<CapturedItemName> . )");
            var visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            var node = visitor.AST;

            Assert.IsTrue(node.Children[0].Token.Name == "CapturingGroup");
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Label");
            Assert.IsTrue(node.Children[0].Children[0].Token.Value == "CapturedItemName");
            Assert.IsTrue(node.Children[0].Children[1].Token.Name == "AnyCharacter");
        }




        [TestMethod]
        public void PEGrammarParser_TerminalReference()
        {
            var rule = this.GetRule("mTerminalReference");
            var root = this.WrapInCapturedGroup("Test", this.RequireEndOfInput(rule));

            var input = new StringInputIterator(@"(?<CapturedItemName>)");
            var visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            var node = visitor.AST;
            Assert.IsTrue(node.Children[0].Token.Name == "TerminalReference");
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Label");
            Assert.IsTrue(node.Children[0].Children[0].Token.Value == "CapturedItemName");



            input = new StringInputIterator(@"(?< CapturedItemName >     )");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            node = visitor.AST;
            Assert.IsTrue(node.Children[0].Token.Name == "TerminalReference");
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Label");
            Assert.IsTrue(node.Children[0].Children[0].Token.Value == "CapturedItemName");



            input = new StringInputIterator(@"(?< CapturedItemName
                                                 \rsc 
                                                 \rn
                                              >     )");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            node = visitor.AST;
            Assert.IsTrue(node.Children[0].Token.Name == "TerminalReference");
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Label");
            Assert.IsTrue(node.Children[0].Children[0].Token.Value == "CapturedItemName");
            Assert.IsTrue(node.Children[0].Children[1].Token.Name == "OptionalFlags");
            Assert.IsTrue(node.Children[0].Children[1].Children[0].Token.Name == "ReplaceBySingleChild");
            Assert.IsTrue(node.Children[0].Children[1].Children[0].Token.Value == @"\rsc");
            Assert.IsTrue(node.Children[0].Children[1].Children[1].Token.Name == "ReplacementNode");
            Assert.IsTrue(node.Children[0].Children[1].Children[1].Token.Value == @"\rn");



            input = new StringInputIterator(@"(?< CapturedItemName 
                                                 // some comment
                                                 \rn // some comment
                                                 \rsc  // some comment
                                              >     )");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            node = visitor.AST;
            Assert.IsTrue(node.Children[0].Token.Name == "TerminalReference");
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Label");
            Assert.IsTrue(node.Children[0].Children[0].Token.Value == "CapturedItemName");
            Assert.IsTrue(node.Children[0].Children[1].Token.Name == "OptionalFlags");
            Assert.IsTrue(node.Children[0].Children[1].Children[0].Token.Name == "ReplacementNode");
            Assert.IsTrue(node.Children[0].Children[1].Children[0].Token.Value == @"\rn");
            Assert.IsTrue(node.Children[0].Children[1].Children[1].Token.Name == "ReplaceBySingleChild");
            Assert.IsTrue(node.Children[0].Children[1].Children[1].Token.Value == @"\rsc");


            input = new StringInputIterator(@"(?<CapturedItemName\rsc\rn>)");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            node = visitor.AST;
            Assert.IsTrue(node.Children[0].Token.Name == "TerminalReference");
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Label");
            Assert.IsTrue(node.Children[0].Children[0].Token.Value == "CapturedItemName");
            Assert.IsTrue(node.Children[0].Children[1].Token.Name == "OptionalFlags");
            Assert.IsTrue(node.Children[0].Children[1].Children[0].Token.Name == "ReplaceBySingleChild");
            Assert.IsTrue(node.Children[0].Children[1].Children[0].Token.Value == @"\rsc");
            Assert.IsTrue(node.Children[0].Children[1].Children[1].Token.Name == "ReplacementNode");
            Assert.IsTrue(node.Children[0].Children[1].Children[1].Token.Value == @"\rn");
        }









        [TestMethod]
        public void PEGrammarParser_ExpressionRoot_Sequence()
        {
            var rule = this.GetRule("mExpressionRoot");
            var root = this.WrapInCapturedGroup("Test", this.RequireEndOfInput(rule));

            var input = new StringInputIterator(@". . .");
            var visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            var node = visitor.AST;
            Assert.IsTrue(node.Children[0].Token.Name == "Sequence");
            Assert.IsTrue(node.Children[0].Children.Count == 3);
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "AnyCharacter");
            Assert.IsTrue(node.Children[0].Children[0].Token.Value == ".");
            Assert.IsTrue(node.Children[0].Children[1].Token.Name == "AnyCharacter");
            Assert.IsTrue(node.Children[0].Children[1].Token.Value == ".");
            Assert.IsTrue(node.Children[0].Children[2].Token.Name == "AnyCharacter");
            Assert.IsTrue(node.Children[0].Children[2].Token.Value == ".");


            input = new StringInputIterator(@". // capture first character
                                            . // capture second character
                                            . // capture third character
                                           ");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            node = visitor.AST;
            Assert.IsTrue(node.Children[0].Token.Name == "Sequence");
            Assert.IsTrue(node.Children[0].Children.Count == 3);
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "AnyCharacter");
            Assert.IsTrue(node.Children[0].Children[0].Token.Value == ".");
            Assert.IsTrue(node.Children[0].Children[1].Token.Name == "AnyCharacter");
            Assert.IsTrue(node.Children[0].Children[1].Token.Value == ".");
            Assert.IsTrue(node.Children[0].Children[2].Token.Name == "AnyCharacter");
            Assert.IsTrue(node.Children[0].Children[2].Token.Value == ".");


            input = new StringInputIterator(@"...");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            node = visitor.AST;
            Assert.IsTrue(node.Children[0].Token.Name == "Sequence");
            Assert.IsTrue(node.Children[0].Children.Count == 3);
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "AnyCharacter");
            Assert.IsTrue(node.Children[0].Children[0].Token.Value == ".");
            Assert.IsTrue(node.Children[0].Children[1].Token.Name == "AnyCharacter");
            Assert.IsTrue(node.Children[0].Children[1].Token.Value == ".");
            Assert.IsTrue(node.Children[0].Children[2].Token.Name == "AnyCharacter");
            Assert.IsTrue(node.Children[0].Children[2].Token.Value == ".");
        }


        [TestMethod]
        public void PEGrammarParser_ExpressionRoot_Sequence_DoReplaceBySingleChildNode()
        {
            var rule = this.GetRule("mExpressionRoot");
            var root = this.WrapInCapturedGroup("Test", this.RequireEndOfInput(rule));

            var input = new StringInputIterator(@".");
            var visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            var node = visitor.AST;
            Assert.IsTrue(node.Children[0].Token.Name == "AnyCharacter");
            Assert.IsTrue(node.Children[0].Token.Value == ".");
        }


        [TestMethod]
        public void PEGrammarParser_ExpressionRoot_PriorityChoice()
        {
            var rule = this.GetRule("mExpressionRoot");
            var root = this.WrapInCapturedGroup("Test", this.RequireEndOfInput(rule));

            // no whitespace
            var input = new StringInputIterator(@"./.");
            var visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            var node = visitor.AST;
            Assert.IsTrue(node.Children[0].Token.Name == "PrioritizedChoice");
            Assert.IsTrue(node.Children[0].Children.Count == 2);
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "AnyCharacter");
            Assert.IsTrue(node.Children[0].Children[0].Token.Value == ".");
            Assert.IsTrue(node.Children[0].Children[1].Token.Name == "AnyCharacter");
            Assert.IsTrue(node.Children[0].Children[1].Token.Value == ".");

            // whitespace
            input = new StringInputIterator(@". / .");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            node = visitor.AST;
            Assert.IsTrue(node.Children[0].Token.Name == "PrioritizedChoice");
            Assert.IsTrue(node.Children[0].Children.Count == 2);
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "AnyCharacter");
            Assert.IsTrue(node.Children[0].Children[0].Token.Value == ".");
            Assert.IsTrue(node.Children[0].Children[1].Token.Name == "AnyCharacter");
            Assert.IsTrue(node.Children[0].Children[1].Token.Value == ".");
        }


        [TestMethod]
        public void PEGrammarParser_ExpressionRoot_Multiple_PriorityChoiceAndSequence()
        {
            var rule = this.GetRule("mExpressionRoot");
            var root = this.WrapInCapturedGroup("Test", this.RequireEndOfInput(rule));

            // no whitespace
            var input = new StringInputIterator(@".../../.../../...");
            var visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            var node = visitor.AST;
            Assert.IsTrue(node.Children[0].Token.Name == "PrioritizedChoice");
            Assert.IsTrue(node.Children[0].Children.Count == 5);
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Sequence");
            Assert.IsTrue(node.Children[0].Children[0].Children.Count == 3);
            Assert.IsTrue(node.Children[0].Children[1].Token.Name == "Sequence");
            Assert.IsTrue(node.Children[0].Children[1].Children.Count == 2);
            Assert.IsTrue(node.Children[0].Children[2].Token.Name == "Sequence");
            Assert.IsTrue(node.Children[0].Children[2].Children.Count == 3);
            Assert.IsTrue(node.Children[0].Children[3].Token.Name == "Sequence");
            Assert.IsTrue(node.Children[0].Children[3].Children.Count == 2);
            Assert.IsTrue(node.Children[0].Children[4].Token.Name == "Sequence");
            Assert.IsTrue(node.Children[0].Children[4].Children.Count == 3);
            
            
            // with whitespace
            input = new StringInputIterator(@"... 
                                            / .. 
                                            /
                                            ... 
                                            /
                                            .. 
                                            / 
                                            ... 
                                           ");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            node = visitor.AST;
            Assert.IsTrue(node.Children[0].Token.Name == "PrioritizedChoice");
            Assert.IsTrue(node.Children[0].Children.Count == 5);
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Sequence");
            Assert.IsTrue(node.Children[0].Children[0].Children.Count == 3);
            Assert.IsTrue(node.Children[0].Children[1].Token.Name == "Sequence");
            Assert.IsTrue(node.Children[0].Children[1].Children.Count == 2);
            Assert.IsTrue(node.Children[0].Children[2].Token.Name == "Sequence");
            Assert.IsTrue(node.Children[0].Children[2].Children.Count == 3);
            Assert.IsTrue(node.Children[0].Children[3].Token.Name == "Sequence");
            Assert.IsTrue(node.Children[0].Children[3].Children.Count == 2);
            Assert.IsTrue(node.Children[0].Children[4].Token.Name == "Sequence");
            Assert.IsTrue(node.Children[0].Children[4].Children.Count == 3);
        }






        [TestMethod]
        public void PEGrammarParser_ExpressionRoot_Terminal_AnyCharacter()
        {
            var rule = this.GetRule("mExpressionRoot");
            var root = this.WrapInCapturedGroup("Test", this.RequireEndOfInput(rule));

            var input = new StringInputIterator(". . .");
            var visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);

            var node = visitor.AST;
            Assert.IsTrue(node.Children[0].Token.Name == "Sequence");
            Assert.IsTrue(node.Children[0].Children.Count == 3);
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "AnyCharacter");
            Assert.IsTrue(node.Children[0].Children[0].Token.Value == ".");
            Assert.IsTrue(node.Children[0].Children[1].Token.Value == ".");
            Assert.IsTrue(node.Children[0].Children[2].Token.Value == ".");
        }


        [TestMethod]
        public void PEGrammarParser_ExpressionRoot_Terminal_CharacterClass()
        {
            var rule = this.GetRule("mExpressionRoot");
            var root = this.WrapInCapturedGroup("Test", this.RequireEndOfInput(rule));

            var input = new StringInputIterator("[a-zA-Z0-9_-]");
            var visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);

            input = new StringInputIterator("[^a-zA-Z0-9_-]");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
        }


        [TestMethod]
        public void PEGrammarParser_ExpressionRoot_Terminal_CodePoint()
        {
            var rule = this.GetRule("mExpressionRoot");
            var root = this.WrapInCapturedGroup("Test", this.RequireEndOfInput(rule));

            var input = new StringInputIterator(@"#0b01010101");
            var visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);

            var node = visitor.AST;
            Assert.IsTrue(node.Children[0].Token.Name == "CodePoint");
            Assert.IsTrue(node.Children[0].Token.Value == "#0b01010101");

            input = new StringInputIterator(@"#2563");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);

            node = visitor.AST;
            Assert.IsTrue(node.Token.Name == "Test");
            Assert.IsTrue(node.Children[0].Token.Name == "CodePoint");
            Assert.IsTrue(node.Children[0].Token.Value == "#2563");


            input = new StringInputIterator(@"#0xffff");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);

            node = visitor.AST;
            Assert.IsTrue(node.Children[0].Token.Name == "CodePoint");
            Assert.IsTrue(node.Children[0].Token.Value == "#0xffff");
        }


        [TestMethod]
        public void PEGrammarParser_ExpressionRoot_Terminal_DynamicBackReference()
        {
            var rule = this.GetRule("mExpressionRoot");
            var root = this.WrapInCapturedGroup("Test", this.RequireEndOfInput(rule));

            var input = new StringInputIterator(@"\k< CapturedLabelVariableName >");
            var visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            var node = visitor.AST;
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Label");
            Assert.IsTrue(node.Children[0].Children[0].Token.Value == "CapturedLabelVariableName");



            input = new StringInputIterator(@"\k<CapturedLabelVariableName>");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            node = visitor.AST;
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Label");
            Assert.IsTrue(node.Children[0].Children[0].Token.Value == "CapturedLabelVariableName");



            input = new StringInputIterator(@"\k< CapturedLabelVariableName[\i] >");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            node = visitor.AST;
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Label");
            Assert.IsTrue(node.Children[0].Children[0].Token.Value == "CapturedLabelVariableName");
            Assert.IsTrue(node.Children[0].Children[1].Token.Name == "CaseSensitive");
            Assert.IsTrue(node.Children[0].Children[1].Token.Value == @"\i");



            input = new StringInputIterator(@"\k< CapturedLabelVariableName   [\i] >");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            node = visitor.AST;
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Label");
            Assert.IsTrue(node.Children[0].Children[0].Token.Value == "CapturedLabelVariableName");
            Assert.IsTrue(node.Children[0].Children[1].Token.Name == "CaseSensitive");
            Assert.IsTrue(node.Children[0].Children[1].Token.Value == @"\i");
        }


        [TestMethod]
        public void PEGrammarParser_ExpressionRoot_FATAL()
        {
            var rule = this.GetRule("mExpressionRoot");
            var root = this.WrapInCapturedGroup("Test", this.RequireEndOfInput(rule));

            var input = new StringInputIterator(@"FaTal< fatal message without single or double quotes >");
            var visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            var node = visitor.AST;
            Assert.IsTrue(node.Children[0].Token.Name == "Fatal");
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Message");
            Assert.IsTrue(node.Children[0].Children[0].Token.Value == "fatal message without single or double quotes");



            input = new StringInputIterator(@"FATAL< 'Fatal with single quotes' >");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            node = visitor.AST;
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Message");
            Assert.IsTrue(node.Children[0].Children[0].Token.Value == "Fatal with single quotes");



            input = new StringInputIterator(@"FATAL<'Fatal with single quotes'>");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            node = visitor.AST;
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Message");
            Assert.IsTrue(node.Children[0].Children[0].Token.Value == "Fatal with single quotes");


            input = new StringInputIterator(@"FATAL<'Fatal\'s escaped single quotes'>");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            node = visitor.AST;
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Message");
            Assert.IsTrue(node.Children[0].Children[0].Token.Value == @"Fatal\'s escaped single quotes");


            input = new StringInputIterator(@"FATAL< ""Fatal with double quotes"" >");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            node = visitor.AST;
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Message");
            Assert.IsTrue(node.Children[0].Children[0].Token.Value == "Fatal with double quotes");



            input = new StringInputIterator(@"FATAL<""Fatal with double quotes"">");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            node = visitor.AST;
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Message");
            Assert.IsTrue(node.Children[0].Children[0].Token.Value == "Fatal with double quotes");


            input = new StringInputIterator(@"FATAL<""Fatal message quoteing \""some other message\"" using escapes"">");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            node = visitor.AST;
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Message");
            Assert.IsTrue(node.Children[0].Children[0].Token.Value == @"Fatal message quoteing \""some other message\"" using escapes");
        }

        [TestMethod]
        public void PEGrammarParser_ExpressionRoot_Literal()
        {
            var rule = this.GetRule("mExpressionRoot");
            var root = this.WrapInCapturedGroup("Test", this.RequireEndOfInput(rule));

            var input = new StringInputIterator(@"'this is some captured text'");
            var visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            var node = visitor.AST;
            Assert.IsTrue(node.Children[0].Token.Name == "Literal");
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "MatchText");
            Assert.IsTrue(node.Children[0].Children[0].Token.Value == "this is some captured text");


            input = new StringInputIterator(@"""this is double quoted captured text""");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            node = visitor.AST;
            Assert.IsTrue(node.Children[0].Token.Name == "Literal");
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "MatchText");
            Assert.IsTrue(node.Children[0].Children[0].Token.Value == "this is double quoted captured text");
        }

        [TestMethod]
        public void PEGrammarParser_ExpressionRoot_RecursionCall()
        {
            var rule = this.GetRule("mExpressionRoot");
            var root = this.WrapInCapturedGroup("Test", this.RequireEndOfInput(rule));

            var input = new StringInputIterator("variaBle023_name _variaBle023_name AAvariaBle023_name");
            var visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);

            AstNode node = visitor.AST;
            Assert.IsTrue(node.Children[0].Token.Name == "Sequence");
            Assert.IsTrue(node.Children[0].Children.Count == 3);
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "RecursionCall");
            Assert.IsTrue(node.Children[0].Children[0].Children[0].Token.Name == "Label");
            Assert.IsTrue(node.Children[0].Children[0].Children[0].Token.Value == "variaBle023_name");
            Assert.IsTrue(node.Children[0].Children[1].Children[0].Token.Value == "_variaBle023_name");
            Assert.IsTrue(node.Children[0].Children[2].Children[0].Token.Value == "AAvariaBle023_name");

        }

        [TestMethod]
        public void PEGrammarParser_ExpressionRoot_WARN()
        {
            var rule = this.GetRule("mExpressionRoot");
            var root = this.WrapInCapturedGroup("Test", this.RequireEndOfInput(rule));

            var input = new StringInputIterator(@"WarN< warning message without single or double quotes >");
            var visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            var node = visitor.AST;
            Assert.IsTrue(node.Children[0].Token.Name == "Warn");
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Message");
            Assert.IsTrue(node.Children[0].Children[0].Token.Value == "warning message without single or double quotes");



            input = new StringInputIterator(@"WARN< 'warning with single quotes' >");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            node = visitor.AST;
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Message");
            Assert.IsTrue(node.Children[0].Children[0].Token.Value == "warning with single quotes");



            input = new StringInputIterator(@"WARN<'warning with single quotes'>");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            node = visitor.AST;
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Message");
            Assert.IsTrue(node.Children[0].Children[0].Token.Value == "warning with single quotes");


            input = new StringInputIterator(@"WARN<'warning\'s escaped single quotes'>");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            node = visitor.AST;
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Message");
            Assert.IsTrue(node.Children[0].Children[0].Token.Value == @"warning\'s escaped single quotes");


            input = new StringInputIterator(@"WARN< ""warning with double quotes"" >");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            node = visitor.AST;
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Message");
            Assert.IsTrue(node.Children[0].Children[0].Token.Value == "warning with double quotes");



            input = new StringInputIterator(@"WARN<""warning with double quotes"">");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            node = visitor.AST;
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Message");
            Assert.IsTrue(node.Children[0].Children[0].Token.Value == "warning with double quotes");


            input = new StringInputIterator(@"WARN<""warning message quoteing \""some other message\"" using escapes"">");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            node = visitor.AST;
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Message");
            Assert.IsTrue(node.Children[0].Children[0].Token.Value == @"warning message quoteing \""some other message\"" using escapes");
        }






        [TestMethod]
        public void PEGrammarParser_ExpressionRoot_LimitingRepetition()
        {
            var rule = this.GetRule("mExpressionRoot");
            var root = this.WrapInCapturedGroup("Test", this.RequireEndOfInput(rule));


            var input = new StringInputIterator(@".{55,77}");
            var visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            var node = visitor.AST;
            Assert.IsTrue(node.Token.Name == "Test");
            Assert.IsTrue(node.Children[0].Token.Name == "Suffix");
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "LimitingRepetition");
            Assert.IsTrue(node.Children[0].Children[0].Children[0].Token.Name == "AnyCharacter");
            Assert.IsTrue(node.Children[0].Children[0].Children[1].Token.Name == "BETWEEN");
            Assert.IsTrue(node.Children[0].Children[0].Children[1].Children[0].Token.Name == "Min");
            Assert.IsTrue(node.Children[0].Children[0].Children[1].Children[0].Token.Value == "55");
            Assert.IsTrue(node.Children[0].Children[0].Children[1].Children[1].Token.Name == "Max");
            Assert.IsTrue(node.Children[0].Children[0].Children[1].Children[1].Token.Value == "77");


            input = new StringInputIterator(@".{ 55 , 77 }");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            node = visitor.AST;
            Assert.IsTrue(node.Token.Name == "Test");
            Assert.IsTrue(node.Children[0].Token.Name == "Suffix");
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "LimitingRepetition");
            Assert.IsTrue(node.Children[0].Children[0].Children[0].Token.Name == "AnyCharacter");
            Assert.IsTrue(node.Children[0].Children[0].Children[1].Token.Name == "BETWEEN");
            Assert.IsTrue(node.Children[0].Children[0].Children[1].Children[0].Token.Name == "Min");
            Assert.IsTrue(node.Children[0].Children[0].Children[1].Children[0].Token.Value == "55");
            Assert.IsTrue(node.Children[0].Children[0].Children[1].Children[1].Token.Name == "Max");
            Assert.IsTrue(node.Children[0].Children[0].Children[1].Children[1].Token.Value == "77");


            input = new StringInputIterator(@".{ , 77 }");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            node = visitor.AST;
            Assert.IsTrue(node.Token.Name == "Test");
            Assert.IsTrue(node.Children[0].Token.Name == "Suffix");
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "LimitingRepetition");
            Assert.IsTrue(node.Children[0].Children[0].Children[0].Token.Name == "AnyCharacter");
            Assert.IsTrue(node.Children[0].Children[0].Children[1].Token.Name == "ATMOST");
            Assert.IsTrue(node.Children[0].Children[0].Children[1].Children.Count == 1);
            Assert.IsTrue(node.Children[0].Children[0].Children[1].Children[0].Token.Name == "Max");
            Assert.IsTrue(node.Children[0].Children[0].Children[1].Children[0].Token.Value == "77");


            input = new StringInputIterator(@".{,77}");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            node = visitor.AST;
            Assert.IsTrue(node.Token.Name == "Test");
            Assert.IsTrue(node.Children[0].Token.Name == "Suffix");
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "LimitingRepetition");
            Assert.IsTrue(node.Children[0].Children[0].Children[0].Token.Name == "AnyCharacter");
            Assert.IsTrue(node.Children[0].Children[0].Children[1].Token.Name == "ATMOST");
            Assert.IsTrue(node.Children[0].Children[0].Children[1].Children.Count == 1);
            Assert.IsTrue(node.Children[0].Children[0].Children[1].Children[0].Token.Name == "Max");
            Assert.IsTrue(node.Children[0].Children[0].Children[1].Children[0].Token.Value == "77");


            input = new StringInputIterator(@".{ 55 ,   }");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            node = visitor.AST;
            Assert.IsTrue(node.Token.Name == "Test");
            Assert.IsTrue(node.Children[0].Token.Name == "Suffix");
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "LimitingRepetition");
            Assert.IsTrue(node.Children[0].Children[0].Children[0].Token.Name == "AnyCharacter");
            Assert.IsTrue(node.Children[0].Children[0].Children[1].Token.Name == "ATLEAST");
            Assert.IsTrue(node.Children[0].Children[0].Children[1].Children[0].Token.Name == "Min");
            Assert.IsTrue(node.Children[0].Children[0].Children[1].Children[0].Token.Value == "55");


            input = new StringInputIterator(@".{55,}");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            node = visitor.AST;
            Assert.IsTrue(node.Token.Name == "Test");
            Assert.IsTrue(node.Children[0].Token.Name == "Suffix");
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "LimitingRepetition");
            Assert.IsTrue(node.Children[0].Children[0].Children[0].Token.Name == "AnyCharacter");
            Assert.IsTrue(node.Children[0].Children[0].Children[1].Token.Name == "ATLEAST");
            Assert.IsTrue(node.Children[0].Children[0].Children[1].Children[0].Token.Name == "Min");
            Assert.IsTrue(node.Children[0].Children[0].Children[1].Children[0].Token.Value == "55");


            input = new StringInputIterator(@".{ 95687 }");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            node = visitor.AST;
            Assert.IsTrue(node.Token.Name == "Test");
            Assert.IsTrue(node.Children[0].Token.Name == "Suffix");
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "LimitingRepetition");
            Assert.IsTrue(node.Children[0].Children[0].Children[0].Token.Name == "AnyCharacter");
            Assert.IsTrue(node.Children[0].Children[0].Children[1].Token.Name == "EXACT");
            Assert.IsTrue(node.Children[0].Children[0].Children[1].Token.Value == "95687");


            input = new StringInputIterator(@".{95687}");
            visitor = new NpegParserVisitor(input);
            root.Accept(visitor);
            Assert.IsTrue(visitor.IsMatch);
            node = visitor.AST;
            Assert.IsTrue(node.Token.Name == "Test");
            Assert.IsTrue(node.Children[0].Token.Name == "Suffix");
            Assert.IsTrue(node.Children[0].Children[0].Token.Name == "LimitingRepetition");
            Assert.IsTrue(node.Children[0].Children[0].Children[0].Token.Name == "AnyCharacter");
            Assert.IsTrue(node.Children[0].Children[0].Children[1].Token.Name == "EXACT");
            Assert.IsTrue(node.Children[0].Children[0].Children[1].Token.Value == "95687");
        }










        private AExpression GetRule(String name)
        {
            var t = typeof(NPEG.GrammarInterpreter.PEGrammar);
            return t.GetMethod(name, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).Invoke(t, new Object[] { }) as AExpression;
        }

        private AExpression OneOrMore(AExpression expression)
        {
            return new NonTerminals.OneOrMore(expression);
        }

        private AExpression RequireEndOfInput(AExpression expression)
        {
            return new NonTerminals.Sequence(expression, new NonTerminals.NotPredicate(new Terminals.AnyCharacter()));
        }

        private AExpression WrapInCapturedGroup(String groupname, AExpression expression)
        {
            return new NonTerminals.CapturingGroup(groupname, expression);
        }

    }
}
