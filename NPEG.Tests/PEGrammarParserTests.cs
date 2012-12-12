using System;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NPEG.Extensions;
using NPEG.GrammarInterpreter;
using NPEG.NonTerminals;
using NPEG.Terminals;

namespace NPEG.Tests
{
	[TestClass]
	public class PEGrammarParserTests
	{
		public TestContext TestContext { get; set; }


		[TestMethod]
		public void PEGrammarParser_Space()
		{
			AExpression rule = OneOrMore(GetRule("mSpace"));
			AExpression root = WrapInCapturedGroup("Test", RequireEndOfInput(rule));


			var bytes = Encoding.UTF8.GetBytes(" \t         \t\t           \t");
			var iterator = new ByteInputIterator(bytes);
			var visitor = new NpegParserVisitor(iterator);

			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
		}


		[TestMethod]
		public void PEGrammarParser_NewLine()
		{
			AExpression rule = OneOrMore(GetRule("mNewLine"));
			AExpression root = WrapInCapturedGroup("Test", RequireEndOfInput(rule));

			// notice only matches newlines of linux/win/mac/ 

			var bytes = Encoding.UTF8.GetBytes("\n\n\r\n\r\r");
			var iterator = new ByteInputIterator(bytes);
			var visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
		}


		[TestMethod]
		public void PEGrammarParser_Comment()
		{
			AExpression rule = OneOrMore(GetRule("mComment"));
			AExpression root = WrapInCapturedGroup("Test", RequireEndOfInput(rule));

			var bytes = Encoding.UTF8.GetBytes(@"//this is a single line comment.");
			var iterator = new ByteInputIterator(bytes);
			var visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);


			bytes = Encoding.UTF8.GetBytes(@"/*this is a multiline comment.*/");
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);


			bytes = Encoding.UTF8.GetBytes(@"/*
                        this 
                        is 
                        a   multiline 
                        comment.
                    */");
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
		}


		[TestMethod]
		public void PEGrammarParser_WhiteSpace()
		{
			AExpression rule = OneOrMore(GetRule("mWhiteSpace"));
			AExpression root = WrapInCapturedGroup("Test", RequireEndOfInput(rule));

			// In the grammar interpreter whitespace is a composite of: 
			// newline, comment, space rules
			var bytes =Encoding.UTF8.GetBytes(
					@"
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
			var iterator = new ByteInputIterator(bytes);
			var visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
		}


		[TestMethod]
		public void PEGrammarParser_Hex()
		{
			AExpression rule = GetRule("mHex");
			AExpression root = WrapInCapturedGroup("Test", RequireEndOfInput(rule));

			var bytes = Encoding.UTF8.GetBytes("0x012345679aaabbbcccdddeeeff");
			var iterator = new ByteInputIterator(bytes);
			var visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);

			bytes = Encoding.UTF8.GetBytes("x012345679aaabbbcccdddeeeff");
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);


			// expected to fail
			bytes = Encoding.UTF8.GetBytes("x012345679ggghhhiiijjjkkklllmmmnnn");
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsFalse(visitor.IsMatch);
		}


		[TestMethod]
		public void PEGrammarParser_Binary()
		{
			AExpression rule = GetRule("mBinary");
			AExpression root = WrapInCapturedGroup("Test", RequireEndOfInput(rule));

			var bytes = Encoding.UTF8.GetBytes("0b01000101010101010");
			var iterator = new ByteInputIterator(bytes);
			var visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);

			bytes = Encoding.UTF8.GetBytes("b01000101010101010");
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);

			// expected to fail
			bytes = Encoding.UTF8.GetBytes("0b0103234234");
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsFalse(visitor.IsMatch);
		}


		[TestMethod]
		public void PEGrammarParser_Label()
		{
			AExpression rule = GetRule("mLabel");
			AExpression root = WrapInCapturedGroup("Test", RequireEndOfInput(rule));

			var input = "variaBle023_name";
			var bytes = Encoding.UTF8.GetBytes(input);
			var iterator = new ByteInputIterator(bytes);
			var visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);

			AstNode node = visitor.AST;
			Assert.IsTrue(node.Children[0].Token.Name == "Label");
			Assert.IsTrue(node.Children[0].Token.Value(iterator) == input);


			bytes = Encoding.UTF8.GetBytes("_variaBle023_name");
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);

			bytes = Encoding.UTF8.GetBytes("AAvariaBle023_name");
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);

			// expected to fail
			bytes = Encoding.UTF8.GetBytes("2invalidvarname");
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsFalse(visitor.IsMatch);
		}


		[TestMethod]
		public void PEGrammarParser_AnyCharacter()
		{
			AExpression rule = GetRule("mAnyCharacter");
			AExpression root = WrapInCapturedGroup("Test", RequireEndOfInput(rule));

			var input = ".";
			var bytes = Encoding.UTF8.GetBytes(input);
			var iterator = new ByteInputIterator(bytes);
			var visitor = new NpegParserVisitor(iterator);

			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);

			AstNode node = visitor.AST;
			Assert.IsTrue(node.Token.Value(iterator) == ".");
		}

		[TestMethod]
		public void PEGrammarParser_CharacterClass()
		{
			AExpression rule = GetRule("mCharacterClass");
			AExpression root = WrapInCapturedGroup("Test", RequireEndOfInput(rule));

			var input = "[a-zA-Z0-9_-]";
			var bytes = Encoding.UTF8.GetBytes(input);
			var iterator = new ByteInputIterator(bytes);
			var visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);

			input = "[^a-zA-Z0-9_-]";
			bytes = Encoding.UTF8.GetBytes(input);
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
		}


		[TestMethod]
		public void PEGrammarParser_CodePoint()
		{
			AExpression rule = GetRule("mCodePoint");
			AExpression root = WrapInCapturedGroup("Test", RequireEndOfInput(rule));

			var input = @"#0b01010101";
			var bytes = Encoding.UTF8.GetBytes(input);
			var iterator = new ByteInputIterator(bytes);
			var visitor = new NpegParserVisitor(iterator);

			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);

			AstNode node = visitor.AST;
			Assert.IsTrue(node.Children[0].Token.Name == "CodePoint");
			Assert.IsTrue(node.Children[0].Token.Value(iterator) == "#0b01010101");

			input = @"#2563";
			bytes = Encoding.UTF8.GetBytes(input);
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);

			node = visitor.AST;
			Assert.IsTrue(node.Token.Name == "Test");
			Assert.IsTrue(node.Children[0].Token.Name == "CodePoint");
			Assert.IsTrue(node.Children[0].Token.Value(iterator) == "#2563");


			input = @"#0xffff";
			bytes = Encoding.UTF8.GetBytes(input);
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);

			node = visitor.AST;
			Assert.IsTrue(node.Children[0].Token.Name == "CodePoint");
			Assert.IsTrue(node.Children[0].Token.Value(iterator) == "#0xffff");
		}


		[TestMethod]
		public void PEGrammarParser_DynamicBackReference()
		{
			AExpression rule = GetRule("mDynamicBackReference");
			AExpression root = WrapInCapturedGroup("Test", RequireEndOfInput(rule));

			var input = @"\k< CapturedLabelVariableName >";
			var bytes = Encoding.UTF8.GetBytes(input);
			var iterator = new ByteInputIterator(bytes);
			var visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			AstNode node = visitor.AST;
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Label");
			Assert.IsTrue(node.Children[0].Children[0].Token.Value(iterator) == "CapturedLabelVariableName");


			input = (@"\k<CapturedLabelVariableName>");
			bytes = Encoding.UTF8.GetBytes(input);
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			node = visitor.AST;
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Label");
			Assert.IsTrue(node.Children[0].Children[0].Token.Value(iterator) == "CapturedLabelVariableName");


			input = (@"\k< CapturedLabelVariableName[\i] >");
			bytes = Encoding.UTF8.GetBytes(input);
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			node = visitor.AST;
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Label");
			Assert.IsTrue(node.Children[0].Children[0].Token.Value(iterator) == "CapturedLabelVariableName");
			Assert.IsTrue(node.Children[0].Children[1].Token.Name == "CaseSensitive");
			Assert.IsTrue(node.Children[0].Children[1].Token.Value(iterator) == @"\i");


			input = (@"\k< CapturedLabelVariableName   [\i] >");
			bytes = Encoding.UTF8.GetBytes(input);
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			node = visitor.AST;
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Label");
			Assert.IsTrue(node.Children[0].Children[0].Token.Value(iterator) == "CapturedLabelVariableName");
			Assert.IsTrue(node.Children[0].Children[1].Token.Name == "CaseSensitive");
			Assert.IsTrue(node.Children[0].Children[1].Token.Value(iterator) == @"\i");
		}


		[TestMethod]
		public void PEGrammarParser_FATAL()
		{
			//It's up to the interpreter to expand: escaped \' and \" to the correct form ' and "

			AExpression rule = GetRule("mFATAL");
			AExpression root = WrapInCapturedGroup("Test", RequireEndOfInput(rule));

			var input = (@"FaTal< fatal message without single or double quotes >");
			var bytes = Encoding.UTF8.GetBytes(input);
			var iterator = new ByteInputIterator(bytes);
			var visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			AstNode node = visitor.AST;
			Assert.IsTrue(node.Children[0].Token.Name == "Fatal");
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Message");
			Assert.IsTrue(node.Children[0].Children[0].Token.Value(iterator) == "fatal message without single or double quotes");


			input = (@"FATAL< 'Fatal with single quotes' >");
			bytes = Encoding.UTF8.GetBytes(input);
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			node = visitor.AST;
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Message");
			Assert.IsTrue(node.Children[0].Children[0].Token.Value(iterator) == "Fatal with single quotes");


			input = (@"FATAL<'Fatal with single quotes'>");
			bytes = Encoding.UTF8.GetBytes(input);
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			node = visitor.AST;
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Message");
			Assert.IsTrue(node.Children[0].Children[0].Token.Value(iterator) == "Fatal with single quotes");


			input = (@"FATAL<'Fatal\'s escaped single quotes'>");
			bytes = Encoding.UTF8.GetBytes(input);
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			node = visitor.AST;
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Message");
			Assert.IsTrue(node.Children[0].Children[0].Token.Value(iterator) == @"Fatal\'s escaped single quotes");


			input = (@"FATAL< ""Fatal with double quotes"" >");
			bytes = Encoding.UTF8.GetBytes(input);
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			node = visitor.AST;
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Message");
			Assert.IsTrue(node.Children[0].Children[0].Token.Value(iterator) == "Fatal with double quotes");


			input = (@"FATAL<""Fatal with double quotes"">");
			bytes = Encoding.UTF8.GetBytes(input);
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			node = visitor.AST;
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Message");
			Assert.IsTrue(node.Children[0].Children[0].Token.Value(iterator) == "Fatal with double quotes");


			input = (@"FATAL<""Fatal message quoteing \""some other message\"" using escapes"">");
			bytes = Encoding.UTF8.GetBytes(input);
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			node = visitor.AST;
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Message");
			Assert.IsTrue(node.Children[0].Children[0].Token.Value(iterator) == @"Fatal message quoteing \""some other message\"" using escapes");
		}


		[TestMethod]
		public void PEGrammarParser_WARN()
		{
			//It's up to the interpreter to expand: escaped \' and \" to the correct form ' and "

			AExpression rule = GetRule("mWARN");
			AExpression root = WrapInCapturedGroup("Test", RequireEndOfInput(rule));

			var input = (@"WarN< warning message without single or double quotes >");
			var bytes = Encoding.UTF8.GetBytes(input);
			var iterator = new ByteInputIterator(bytes);
			var visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			AstNode node = visitor.AST;
			Assert.IsTrue(node.Children[0].Token.Name == "Warn");
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Message");
			Assert.IsTrue(node.Children[0].Children[0].Token.Value(iterator) == "warning message without single or double quotes");


			input = (@"WARN< 'warning with single quotes' >");
			bytes = Encoding.UTF8.GetBytes(input);
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			node = visitor.AST;
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Message");
			Assert.IsTrue(node.Children[0].Children[0].Token.Value(iterator) == "warning with single quotes");


			input = (@"WARN<'warning with single quotes'>");
			bytes = Encoding.UTF8.GetBytes(input);
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			node = visitor.AST;
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Message");
			Assert.IsTrue(node.Children[0].Children[0].Token.Value(iterator) == "warning with single quotes");


			input = (@"WARN<'warning\'s escaped single quotes'>");
			bytes = Encoding.UTF8.GetBytes(input);
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			node = visitor.AST;
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Message");
			Assert.IsTrue(node.Children[0].Children[0].Token.Value(iterator) == @"warning\'s escaped single quotes");


			input = (@"WARN< ""warning with double quotes"" >");
			bytes = Encoding.UTF8.GetBytes(input);
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			node = visitor.AST;
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Message");
			Assert.IsTrue(node.Children[0].Children[0].Token.Value(iterator) == "warning with double quotes");


			input = (@"WARN<""warning with double quotes"">");
			bytes = Encoding.UTF8.GetBytes(input);
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			node = visitor.AST;
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Message");
			Assert.IsTrue(node.Children[0].Children[0].Token.Value(iterator) == "warning with double quotes");


			input = (@"WARN<""warning message quoteing \""some other message\"" using escapes"">");
			bytes = Encoding.UTF8.GetBytes(input);
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			node = visitor.AST;
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Message");
			Assert.IsTrue(node.Children[0].Children[0].Token.Value(iterator) ==
			              @"warning message quoteing \""some other message\"" using escapes");
		}


		[TestMethod]
		public void PEGrammarParser_Literal()
		{
			AExpression rule = GetRule("mLiteral");
			AExpression root = WrapInCapturedGroup("Test", RequireEndOfInput(rule));

			var input = (@"'this is some captured text'");
			var bytes = Encoding.UTF8.GetBytes(input);
			var iterator = new ByteInputIterator(bytes);
			var visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			AstNode node = visitor.AST;
			Assert.IsTrue(node.Children[0].Token.Name == "Literal");
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "MatchText");
			Assert.IsTrue(node.Children[0].Children[0].Token.Value(iterator) == "this is some captured text");


			input = (@"""this is double quoted captured text""");
			bytes = Encoding.UTF8.GetBytes(input);
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			node = visitor.AST;
			Assert.IsTrue(node.Children[0].Token.Name == "Literal");
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "MatchText");
			Assert.IsTrue(node.Children[0].Children[0].Token.Value(iterator) == "this is double quoted captured text");
		}


		[TestMethod]
		public void PEGrammarParser_CapturingGroup()
		{
			AExpression rule = GetRule("mExpressionRoot");
			AExpression root = WrapInCapturedGroup("Test", RequireEndOfInput(rule));

			var input = (@"(?<CapturedItemName> . )");
			var bytes = Encoding.UTF8.GetBytes(input);
			var iterator = new ByteInputIterator(bytes);
			var visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			AstNode node = visitor.AST;

			Assert.IsTrue(node.Children[0].Token.Name == "CapturingGroup");
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Label");
			Assert.IsTrue(node.Children[0].Children[0].Token.Value(iterator) == "CapturedItemName");
			Assert.IsTrue(node.Children[0].Children[1].Token.Name == "AnyCharacter");
		}


		[TestMethod]
		public void PEGrammarParser_TerminalReference()
		{
			AExpression rule = GetRule("mTerminalReference");
			AExpression root = WrapInCapturedGroup("Test", RequireEndOfInput(rule));

			var input = (@"(?<CapturedItemName>)");
			var bytes = Encoding.UTF8.GetBytes(input);
			var iterator = new ByteInputIterator(bytes);
			var visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			AstNode node = visitor.AST;
			Assert.IsTrue(node.Children[0].Token.Name == "TerminalReference");
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Label");
			Assert.IsTrue(node.Children[0].Children[0].Token.Value(iterator) == "CapturedItemName");


			input = (@"(?< CapturedItemName >     )");
			bytes = Encoding.UTF8.GetBytes(input);
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			node = visitor.AST;
			Assert.IsTrue(node.Children[0].Token.Name == "TerminalReference");
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Label");
			Assert.IsTrue(node.Children[0].Children[0].Token.Value(iterator) == "CapturedItemName");


			input = @"(?< CapturedItemName
                                                 \rsc 
                                                 \rn
                                              >     )";
			bytes = Encoding.UTF8.GetBytes(input);
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			node = visitor.AST;
			Assert.IsTrue(node.Children[0].Token.Name == "TerminalReference");
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Label");
			Assert.IsTrue(node.Children[0].Children[0].Token.Value(iterator) == "CapturedItemName");
			Assert.IsTrue(node.Children[0].Children[1].Token.Name == "OptionalFlags");
			Assert.IsTrue(node.Children[0].Children[1].Children[0].Token.Name == "ReplaceBySingleChild");
			Assert.IsTrue(node.Children[0].Children[1].Children[0].Token.Value(iterator) == @"\rsc");
			Assert.IsTrue(node.Children[0].Children[1].Children[1].Token.Name == "ReplacementNode");
			Assert.IsTrue(node.Children[0].Children[1].Children[1].Token.Value(iterator) == @"\rn");


			input =
					@"(?< CapturedItemName 
                                                 // some comment
                                                 \rn // some comment
                                                 \rsc  // some comment
                                              >     )";
			bytes = Encoding.UTF8.GetBytes(input);
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			node = visitor.AST;
			Assert.IsTrue(node.Children[0].Token.Name == "TerminalReference");
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Label");
			Assert.IsTrue(node.Children[0].Children[0].Token.Value(iterator) == "CapturedItemName");
			Assert.IsTrue(node.Children[0].Children[1].Token.Name == "OptionalFlags");
			Assert.IsTrue(node.Children[0].Children[1].Children[0].Token.Name == "ReplacementNode");
			Assert.IsTrue(node.Children[0].Children[1].Children[0].Token.Value(iterator) == @"\rn");
			Assert.IsTrue(node.Children[0].Children[1].Children[1].Token.Name == "ReplaceBySingleChild");
			Assert.IsTrue(node.Children[0].Children[1].Children[1].Token.Value(iterator) == @"\rsc");


			input = (@"(?<CapturedItemName\rsc\rn>)");
			bytes = Encoding.UTF8.GetBytes(input);
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			node = visitor.AST;
			Assert.IsTrue(node.Children[0].Token.Name == "TerminalReference");
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Label");
			Assert.IsTrue(node.Children[0].Children[0].Token.Value(iterator) == "CapturedItemName");
			Assert.IsTrue(node.Children[0].Children[1].Token.Name == "OptionalFlags");
			Assert.IsTrue(node.Children[0].Children[1].Children[0].Token.Name == "ReplaceBySingleChild");
			Assert.IsTrue(node.Children[0].Children[1].Children[0].Token.Value(iterator) == @"\rsc");
			Assert.IsTrue(node.Children[0].Children[1].Children[1].Token.Name == "ReplacementNode");
			Assert.IsTrue(node.Children[0].Children[1].Children[1].Token.Value(iterator) == @"\rn");
		}


		[TestMethod]
		public void PEGrammarParser_ExpressionRoot_Sequence()
		{
			AExpression rule = GetRule("mExpressionRoot");
			AExpression root = WrapInCapturedGroup("Test", RequireEndOfInput(rule));

			var input = (@". . .");
			var bytes = Encoding.UTF8.GetBytes(input);
			var iterator = new ByteInputIterator(bytes);
			var visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			AstNode node = visitor.AST;
			Assert.IsTrue(node.Children[0].Token.Name == "Sequence");
			Assert.IsTrue(node.Children[0].Children.Count == 3);
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "AnyCharacter");
			Assert.IsTrue(node.Children[0].Children[0].Token.Value(iterator) == ".");
			Assert.IsTrue(node.Children[0].Children[1].Token.Name == "AnyCharacter");
			Assert.IsTrue(node.Children[0].Children[1].Token.Value(iterator) == ".");
			Assert.IsTrue(node.Children[0].Children[2].Token.Name == "AnyCharacter");
			Assert.IsTrue(node.Children[0].Children[2].Token.Value(iterator) == ".");


			input = @". // capture first character
                                            . // capture second character
                                            . // capture third character
                                           ";
			bytes = Encoding.UTF8.GetBytes(input);
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			node = visitor.AST;
			Assert.IsTrue(node.Children[0].Token.Name == "Sequence");
			Assert.IsTrue(node.Children[0].Children.Count == 3);
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "AnyCharacter");
			Assert.IsTrue(node.Children[0].Children[0].Token.Value(iterator) == ".");
			Assert.IsTrue(node.Children[0].Children[1].Token.Name == "AnyCharacter");
			Assert.IsTrue(node.Children[0].Children[1].Token.Value(iterator) == ".");
			Assert.IsTrue(node.Children[0].Children[2].Token.Name == "AnyCharacter");
			Assert.IsTrue(node.Children[0].Children[2].Token.Value(iterator) == ".");


			input = (@"...");
			bytes = Encoding.UTF8.GetBytes(input);
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			node = visitor.AST;
			Assert.IsTrue(node.Children[0].Token.Name == "Sequence");
			Assert.IsTrue(node.Children[0].Children.Count == 3);
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "AnyCharacter");
			Assert.IsTrue(node.Children[0].Children[0].Token.Value(iterator) == ".");
			Assert.IsTrue(node.Children[0].Children[1].Token.Name == "AnyCharacter");
			Assert.IsTrue(node.Children[0].Children[1].Token.Value(iterator) == ".");
			Assert.IsTrue(node.Children[0].Children[2].Token.Name == "AnyCharacter");
			Assert.IsTrue(node.Children[0].Children[2].Token.Value(iterator) == ".");
		}


		[TestMethod]
		public void PEGrammarParser_ExpressionRoot_Sequence_DoReplaceBySingleChildNode()
		{
			AExpression rule = GetRule("mExpressionRoot");
			AExpression root = WrapInCapturedGroup("Test", RequireEndOfInput(rule));

			var input = (@".");
			var bytes = Encoding.UTF8.GetBytes(input);
			var iterator = new ByteInputIterator(bytes);
			var visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			AstNode node = visitor.AST;
			Assert.IsTrue(node.Children[0].Token.Name == "AnyCharacter");
			Assert.IsTrue(node.Children[0].Token.Value(iterator) == ".");
		}


		[TestMethod]
		public void PEGrammarParser_ExpressionRoot_PriorityChoice()
		{
			AExpression rule = GetRule("mExpressionRoot");
			AExpression root = WrapInCapturedGroup("Test", RequireEndOfInput(rule));

			// no whitespace
			var input = (@"./.");
			var bytes = Encoding.UTF8.GetBytes(input);
			var iterator = new ByteInputIterator(bytes);
			var visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			AstNode node = visitor.AST;
			Assert.IsTrue(node.Children[0].Token.Name == "PrioritizedChoice");
			Assert.IsTrue(node.Children[0].Children.Count == 2);
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "AnyCharacter");
			Assert.IsTrue(node.Children[0].Children[0].Token.Value(iterator) == ".");
			Assert.IsTrue(node.Children[0].Children[1].Token.Name == "AnyCharacter");
			Assert.IsTrue(node.Children[0].Children[1].Token.Value(iterator) == ".");

			// whitespace
			input = (@". / .");
			bytes = Encoding.UTF8.GetBytes(input);
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			node = visitor.AST;
			Assert.IsTrue(node.Children[0].Token.Name == "PrioritizedChoice");
			Assert.IsTrue(node.Children[0].Children.Count == 2);
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "AnyCharacter");
			Assert.IsTrue(node.Children[0].Children[0].Token.Value(iterator) == ".");
			Assert.IsTrue(node.Children[0].Children[1].Token.Name == "AnyCharacter");
			Assert.IsTrue(node.Children[0].Children[1].Token.Value(iterator) == ".");
		}


		[TestMethod]
		public void PEGrammarParser_ExpressionRoot_Multiple_PriorityChoiceAndSequence()
		{
			AExpression rule = GetRule("mExpressionRoot");
			AExpression root = WrapInCapturedGroup("Test", RequireEndOfInput(rule));

			// no whitespace
			var input = (@".../../.../../...");
			var bytes = Encoding.UTF8.GetBytes(input);
			var iterator = new ByteInputIterator(bytes);
			var visitor = new NpegParserVisitor(iterator);

			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			AstNode node = visitor.AST;
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
			input =
					@"... 
                                            / .. 
                                            /
                                            ... 
                                            /
                                            .. 
                                            / 
                                            ... 
                                           ";
			bytes = Encoding.UTF8.GetBytes(input);
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
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
			AExpression rule = GetRule("mExpressionRoot");
			AExpression root = WrapInCapturedGroup("Test", RequireEndOfInput(rule));

			var input = (". . .");
			var bytes = Encoding.UTF8.GetBytes(input);
			var iterator = new ByteInputIterator(bytes);
			var visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);

			AstNode node = visitor.AST;
			Assert.IsTrue(node.Children[0].Token.Name == "Sequence");
			Assert.IsTrue(node.Children[0].Children.Count == 3);
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "AnyCharacter");
			Assert.IsTrue(node.Children[0].Children[0].Token.Value(iterator) == ".");
			Assert.IsTrue(node.Children[0].Children[1].Token.Value(iterator) == ".");
			Assert.IsTrue(node.Children[0].Children[2].Token.Value(iterator) == ".");
		}


		[TestMethod]
		public void PEGrammarParser_ExpressionRoot_Terminal_CharacterClass()
		{
			AExpression rule = GetRule("mExpressionRoot");
			AExpression root = WrapInCapturedGroup("Test", RequireEndOfInput(rule));

			var input = ("[a-zA-Z0-9_-]");
			var bytes = Encoding.UTF8.GetBytes(input);
			var iterator = new ByteInputIterator(bytes);
			var visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);

			input = ("[^a-zA-Z0-9_-]");
			bytes = Encoding.UTF8.GetBytes(input);
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
		}


		[TestMethod]
		public void PEGrammarParser_ExpressionRoot_Terminal_CodePoint()
		{
			AExpression rule = GetRule("mExpressionRoot");
			AExpression root = WrapInCapturedGroup("Test", RequireEndOfInput(rule));

			var input = (@"#0b01010101");
			var bytes = Encoding.UTF8.GetBytes(input);
			var iterator = new ByteInputIterator(bytes);
			var visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);

			AstNode node = visitor.AST;
			Assert.IsTrue(node.Children[0].Token.Name == "CodePoint");
			Assert.IsTrue(node.Children[0].Token.Value(iterator) == "#0b01010101");

			input = (@"#2563");
			bytes = Encoding.UTF8.GetBytes(input);
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);

			node = visitor.AST;
			Assert.IsTrue(node.Token.Name == "Test");
			Assert.IsTrue(node.Children[0].Token.Name == "CodePoint");
			Assert.IsTrue(node.Children[0].Token.Value(iterator) == "#2563");


			input = (@"#0xffff");
			bytes = Encoding.UTF8.GetBytes(input);
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);

			node = visitor.AST;
			Assert.IsTrue(node.Children[0].Token.Name == "CodePoint");
			Assert.IsTrue(node.Children[0].Token.Value(iterator) == "#0xffff");
		}


		[TestMethod]
		public void PEGrammarParser_ExpressionRoot_Terminal_DynamicBackReference()
		{
			AExpression rule = GetRule("mExpressionRoot");
			AExpression root = WrapInCapturedGroup("Test", RequireEndOfInput(rule));

			var input = (@"\k< CapturedLabelVariableName >");
			var bytes = Encoding.UTF8.GetBytes(input);
			var iterator = new ByteInputIterator(bytes);
			var visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			AstNode node = visitor.AST;
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Label");
			Assert.IsTrue(node.Children[0].Children[0].Token.Value(iterator) == "CapturedLabelVariableName");


			input = (@"\k<CapturedLabelVariableName>");
			bytes = Encoding.UTF8.GetBytes(input);
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			node = visitor.AST;
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Label");
			Assert.IsTrue(node.Children[0].Children[0].Token.Value(iterator) == "CapturedLabelVariableName");


			input = (@"\k< CapturedLabelVariableName[\i] >");
			bytes = Encoding.UTF8.GetBytes(input);
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			node = visitor.AST;
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Label");
			Assert.IsTrue(node.Children[0].Children[0].Token.Value(iterator) == "CapturedLabelVariableName");
			Assert.IsTrue(node.Children[0].Children[1].Token.Name == "CaseSensitive");
			Assert.IsTrue(node.Children[0].Children[1].Token.Value(iterator) == @"\i");


			input = (@"\k< CapturedLabelVariableName   [\i] >");
			bytes = Encoding.UTF8.GetBytes(input);
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			node = visitor.AST;
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Label");
			Assert.IsTrue(node.Children[0].Children[0].Token.Value(iterator) == "CapturedLabelVariableName");
			Assert.IsTrue(node.Children[0].Children[1].Token.Name == "CaseSensitive");
			Assert.IsTrue(node.Children[0].Children[1].Token.Value(iterator) == @"\i");
		}


		[TestMethod]
		public void PEGrammarParser_ExpressionRoot_FATAL()
		{
			AExpression rule = GetRule("mExpressionRoot");
			AExpression root = WrapInCapturedGroup("Test", RequireEndOfInput(rule));

			var input = (@"FaTal< fatal message without single or double quotes >");
			var bytes = Encoding.UTF8.GetBytes(input);
			var iterator = new ByteInputIterator(bytes);
			var visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			AstNode node = visitor.AST;
			Assert.IsTrue(node.Children[0].Token.Name == "Fatal");
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Message");
			Assert.IsTrue(node.Children[0].Children[0].Token.Value(iterator) == "fatal message without single or double quotes");


			input = (@"FATAL< 'Fatal with single quotes' >");
			bytes = Encoding.UTF8.GetBytes(input);
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			node = visitor.AST;
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Message");
			Assert.IsTrue(node.Children[0].Children[0].Token.Value(iterator) == "Fatal with single quotes");


			input = (@"FATAL<'Fatal with single quotes'>");
			bytes = Encoding.UTF8.GetBytes(input);
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			node = visitor.AST;
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Message");
			Assert.IsTrue(node.Children[0].Children[0].Token.Value(iterator) == "Fatal with single quotes");


			input = (@"FATAL<'Fatal\'s escaped single quotes'>");
			bytes = Encoding.UTF8.GetBytes(input);
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			node = visitor.AST;
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Message");
			Assert.IsTrue(node.Children[0].Children[0].Token.Value(iterator) == @"Fatal\'s escaped single quotes");


			input = (@"FATAL< ""Fatal with double quotes"" >");
			bytes = Encoding.UTF8.GetBytes(input);
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			node = visitor.AST;
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Message");
			Assert.IsTrue(node.Children[0].Children[0].Token.Value(iterator) == "Fatal with double quotes");


			input = (@"FATAL<""Fatal with double quotes"">");
			bytes = Encoding.UTF8.GetBytes(input);
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			node = visitor.AST;
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Message");
			Assert.IsTrue(node.Children[0].Children[0].Token.Value(iterator) == "Fatal with double quotes");


			input = (@"FATAL<""Fatal message quoteing \""some other message\"" using escapes"">");
			bytes = Encoding.UTF8.GetBytes(input);
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			node = visitor.AST;
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Message");
			Assert.IsTrue(node.Children[0].Children[0].Token.Value(iterator) == @"Fatal message quoteing \""some other message\"" using escapes");
		}

		[TestMethod]
		public void PEGrammarParser_ExpressionRoot_Literal()
		{
			AExpression rule = GetRule("mExpressionRoot");
			AExpression root = WrapInCapturedGroup("Test", RequireEndOfInput(rule));

			var input = (@"'this is some captured text'");
			var bytes = Encoding.UTF8.GetBytes(input);
			var iterator = new ByteInputIterator(bytes);
			var visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			AstNode node = visitor.AST;
			Assert.IsTrue(node.Children[0].Token.Name == "Literal");
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "MatchText");
			Assert.IsTrue(node.Children[0].Children[0].Token.Value(iterator) == "this is some captured text");


			input = (@"""this is double quoted captured text""");
			bytes = Encoding.UTF8.GetBytes(input);
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			node = visitor.AST;
			Assert.IsTrue(node.Children[0].Token.Name == "Literal");
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "MatchText");
			Assert.IsTrue(node.Children[0].Children[0].Token.Value(iterator) == "this is double quoted captured text");
		}

		[TestMethod]
		public void PEGrammarParser_ExpressionRoot_RecursionCall()
		{
			AExpression rule = GetRule("mExpressionRoot");
			AExpression root = WrapInCapturedGroup("Test", RequireEndOfInput(rule));

			var input = ("variaBle023_name _variaBle023_name AAvariaBle023_name");
			var bytes = Encoding.UTF8.GetBytes(input);
			var iterator = new ByteInputIterator(bytes);
			var visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);

			AstNode node = visitor.AST;
			Assert.IsTrue(node.Children[0].Token.Name == "Sequence");
			Assert.IsTrue(node.Children[0].Children.Count == 3);
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "RecursionCall");
			Assert.IsTrue(node.Children[0].Children[0].Children[0].Token.Name == "Label");
			Assert.IsTrue(node.Children[0].Children[0].Children[0].Token.Value(iterator) == "variaBle023_name");
			Assert.IsTrue(node.Children[0].Children[1].Children[0].Token.Value(iterator) == "_variaBle023_name");
			Assert.IsTrue(node.Children[0].Children[2].Children[0].Token.Value(iterator) == "AAvariaBle023_name");
		}

		[TestMethod]
		public void PEGrammarParser_ExpressionRoot_WARN()
		{
			AExpression rule = GetRule("mExpressionRoot");
			AExpression root = WrapInCapturedGroup("Test", RequireEndOfInput(rule));

			var input = (@"WarN< warning message without single or double quotes >");
			var bytes = Encoding.UTF8.GetBytes(input);
			var iterator = new ByteInputIterator(bytes);
			var visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			AstNode node = visitor.AST;
			Assert.IsTrue(node.Children[0].Token.Name == "Warn");
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Message");
			Assert.IsTrue(node.Children[0].Children[0].Token.Value(iterator) == "warning message without single or double quotes");


			input = (@"WARN< 'warning with single quotes' >");
			bytes = Encoding.UTF8.GetBytes(input);
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			node = visitor.AST;
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Message");
			Assert.IsTrue(node.Children[0].Children[0].Token.Value(iterator) == "warning with single quotes");


			input = (@"WARN<'warning with single quotes'>");
			bytes = Encoding.UTF8.GetBytes(input);
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			node = visitor.AST;
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Message");
			Assert.IsTrue(node.Children[0].Children[0].Token.Value(iterator) == "warning with single quotes");


			input = (@"WARN<'warning\'s escaped single quotes'>");
			bytes = Encoding.UTF8.GetBytes(input);
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			node = visitor.AST;
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Message");
			Assert.IsTrue(node.Children[0].Children[0].Token.Value(iterator) == @"warning\'s escaped single quotes");


			input = (@"WARN< ""warning with double quotes"" >");
			bytes = Encoding.UTF8.GetBytes(input);
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			node = visitor.AST;
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Message");
			Assert.IsTrue(node.Children[0].Children[0].Token.Value(iterator) == "warning with double quotes");


			input = (@"WARN<""warning with double quotes"">");
			bytes = Encoding.UTF8.GetBytes(input);
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			node = visitor.AST;
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Message");
			Assert.IsTrue(node.Children[0].Children[0].Token.Value(iterator) == "warning with double quotes");


			input = (@"WARN<""warning message quoteing \""some other message\"" using escapes"">");
			bytes = Encoding.UTF8.GetBytes(input);
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			node = visitor.AST;
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Message");
			Assert.IsTrue(node.Children[0].Children[0].Token.Value(iterator) == @"warning message quoteing \""some other message\"" using escapes");
		}


		[TestMethod]
		public void PEGrammarParser_ExpressionRoot_LimitingRepetition()
		{
			AExpression rule = GetRule("mExpressionRoot");
			AExpression root = WrapInCapturedGroup("Test", RequireEndOfInput(rule));


			var input = (@".{55,77}");
			var bytes = Encoding.UTF8.GetBytes(input);
			var iterator = new ByteInputIterator(bytes);
			var visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			AstNode node = visitor.AST;
			Assert.IsTrue(node.Token.Name == "Test");
			Assert.IsTrue(node.Children[0].Token.Name == "Suffix");
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "LimitingRepetition");
			Assert.IsTrue(node.Children[0].Children[0].Children[0].Token.Name == "AnyCharacter");
			Assert.IsTrue(node.Children[0].Children[0].Children[1].Token.Name == "BETWEEN");
			Assert.IsTrue(node.Children[0].Children[0].Children[1].Children[0].Token.Name == "Min");
			Assert.IsTrue(node.Children[0].Children[0].Children[1].Children[0].Token.Value(iterator) == "55");
			Assert.IsTrue(node.Children[0].Children[0].Children[1].Children[1].Token.Name == "Max");
			Assert.IsTrue(node.Children[0].Children[0].Children[1].Children[1].Token.Value(iterator) == "77");


			input = (@".{ 55 , 77 }");
			bytes = Encoding.UTF8.GetBytes(input);
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			node = visitor.AST;
			Assert.IsTrue(node.Token.Name == "Test");
			Assert.IsTrue(node.Children[0].Token.Name == "Suffix");
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "LimitingRepetition");
			Assert.IsTrue(node.Children[0].Children[0].Children[0].Token.Name == "AnyCharacter");
			Assert.IsTrue(node.Children[0].Children[0].Children[1].Token.Name == "BETWEEN");
			Assert.IsTrue(node.Children[0].Children[0].Children[1].Children[0].Token.Name == "Min");
			Assert.IsTrue(node.Children[0].Children[0].Children[1].Children[0].Token.Value(iterator) == "55");
			Assert.IsTrue(node.Children[0].Children[0].Children[1].Children[1].Token.Name == "Max");
			Assert.IsTrue(node.Children[0].Children[0].Children[1].Children[1].Token.Value(iterator) == "77");


			input = (@".{ , 77 }");
			bytes = Encoding.UTF8.GetBytes(input);
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
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
			Assert.IsTrue(node.Children[0].Children[0].Children[1].Children[0].Token.Value(iterator) == "77");


			input = (@".{,77}");
			bytes = Encoding.UTF8.GetBytes(input);
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
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
			Assert.IsTrue(node.Children[0].Children[0].Children[1].Children[0].Token.Value(iterator) == "77");


			input = (@".{ 55 ,   }");
			bytes = Encoding.UTF8.GetBytes(input);
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			node = visitor.AST;
			Assert.IsTrue(node.Token.Name == "Test");
			Assert.IsTrue(node.Children[0].Token.Name == "Suffix");
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "LimitingRepetition");
			Assert.IsTrue(node.Children[0].Children[0].Children[0].Token.Name == "AnyCharacter");
			Assert.IsTrue(node.Children[0].Children[0].Children[1].Token.Name == "ATLEAST");
			Assert.IsTrue(node.Children[0].Children[0].Children[1].Children[0].Token.Name == "Min");
			Assert.IsTrue(node.Children[0].Children[0].Children[1].Children[0].Token.Value(iterator) == "55");


			input = (@".{55,}");
			bytes = Encoding.UTF8.GetBytes(input);
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			node = visitor.AST;
			Assert.IsTrue(node.Token.Name == "Test");
			Assert.IsTrue(node.Children[0].Token.Name == "Suffix");
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "LimitingRepetition");
			Assert.IsTrue(node.Children[0].Children[0].Children[0].Token.Name == "AnyCharacter");
			Assert.IsTrue(node.Children[0].Children[0].Children[1].Token.Name == "ATLEAST");
			Assert.IsTrue(node.Children[0].Children[0].Children[1].Children[0].Token.Name == "Min");
			Assert.IsTrue(node.Children[0].Children[0].Children[1].Children[0].Token.Value(iterator) == "55");


			input = (@".{ 95687 }");
			bytes = Encoding.UTF8.GetBytes(input);
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			node = visitor.AST;
			Assert.IsTrue(node.Token.Name == "Test");
			Assert.IsTrue(node.Children[0].Token.Name == "Suffix");
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "LimitingRepetition");
			Assert.IsTrue(node.Children[0].Children[0].Children[0].Token.Name == "AnyCharacter");
			Assert.IsTrue(node.Children[0].Children[0].Children[1].Token.Name == "EXACT");
			Assert.IsTrue(node.Children[0].Children[0].Children[1].Token.Value(iterator) == "95687");


			input = (@".{95687}");
			bytes = Encoding.UTF8.GetBytes(input);
			iterator = new ByteInputIterator(bytes);
			visitor = new NpegParserVisitor(iterator);
			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			node = visitor.AST;
			Assert.IsTrue(node.Token.Name == "Test");
			Assert.IsTrue(node.Children[0].Token.Name == "Suffix");
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "LimitingRepetition");
			Assert.IsTrue(node.Children[0].Children[0].Children[0].Token.Name == "AnyCharacter");
			Assert.IsTrue(node.Children[0].Children[0].Children[1].Token.Name == "EXACT");
			Assert.IsTrue(node.Children[0].Children[0].Children[1].Token.Value(iterator) == "95687");
		}


		private AExpression GetRule(String name)
		{
			Type t = typeof (PEGrammar);
			return t.GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic).Invoke(t, new Object[] {}) as AExpression;
		}

		private AExpression OneOrMore(AExpression expression)
		{
			return new OneOrMore(expression);
		}

		private AExpression RequireEndOfInput(AExpression expression)
		{
			return new Sequence(expression, new NotPredicate(new AnyCharacter()));
		}

		private AExpression WrapInCapturedGroup(String groupname, AExpression expression)
		{
			return new CapturingGroup(groupname, expression);
		}
	}
}