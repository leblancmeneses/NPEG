using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NPEG.ApplicationExceptions;
using NPEG.GrammarInterpreter;

namespace NPEG.Tests
{
	[TestClass]
	public class GrammarInterpreterTests
	{
		public TestContext TestContext { get; set; }

		[TestMethod]
		public void PEGrammar_Literal()
		{
			string input = "hello world";
			AExpression caseSensitive = PEGrammar.Load(@"(?<Expression>): 'Hello World';");

			var visitor = new NpegParserVisitor(new StringInputIterator(input));
			caseSensitive.Accept(visitor);
			Assert.IsFalse(visitor.IsMatch);

			AExpression notCaseSensitive = PEGrammar.Load(@"(?<Expression>): 'Hello World'\i;");
			visitor = new NpegParserVisitor(new StringInputIterator(input));
			notCaseSensitive.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			AstNode node = visitor.AST;
			Assert.IsTrue(node.Token.Name == "Expression");
			Assert.IsTrue(node.Token.Value == input);


			// not sure if it would be better to use verbatim identifier @"" for escaping
			// escape back slash inside double quotes
			input = @"\";
			AExpression escape = PEGrammar.Load(@"(?<Literal>): ""\\"";");
			visitor = new NpegParserVisitor(new StringInputIterator(input));
			escape.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			Assert.IsTrue(@"\" == visitor.AST.Token.Value);

			input = @"\";
			escape = PEGrammar.Load(@"(?<Literal>): '\\';");
			visitor = new NpegParserVisitor(new StringInputIterator(input));
			escape.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			Assert.IsTrue(@"\" == visitor.AST.Token.Value);
		}


		[TestMethod]
		[Timeout(2000)]
		[ExpectedException(typeof (InfiniteLoopDetectedException))]
		public void PEGrammar_ZeroOrMore_InfiniteLoopTest()
		{
			string input = "";
			AExpression caseSensitive = PEGrammar.Load(@"(?<Expression>): (.*)*;");

			var visitor = new NpegParserVisitor(new StringInputIterator(input));
			caseSensitive.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
		}

		[TestMethod]
		[Timeout(2000)]
		[ExpectedException(typeof (InfiniteLoopDetectedException))]
		public void PEGrammar_LimitingRepetition_InfiniteLoopTest()
		{
			string input = "";
			AExpression caseSensitive = PEGrammar.Load(@"(?<Expression>): (.*){0,};");

			var visitor = new NpegParserVisitor(new StringInputIterator(input));
			caseSensitive.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
		}

		[TestMethod]
		[Timeout(2000)]
		[ExpectedException(typeof (InfiniteLoopDetectedException))]
		public void PEGrammar_OneOrMore_InfiniteLoopTest()
		{
			string input = "";
			AExpression caseSensitive = PEGrammar.Load(@"(?<Expression>): (.*)+;");

			var visitor = new NpegParserVisitor(new StringInputIterator(input));
			caseSensitive.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
		}

		[TestMethod]
		public void PEGrammar_OneOrMore_WithPredicate_DoesNotCauseInfiniteLoop()
		{
			string input = "";
			AExpression caseSensitive = PEGrammar.Load(@"(?<Expression>): (.*)+;");

			var visitor = new NpegParserVisitor(new StringInputIterator(input));
			caseSensitive.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
		}


		// Root node must always be a capturing group.
		[TestMethod]
		public void PEGrammar_LimitingRepetition()
		{
			String grammar =
				@"
                                (?<ThreeDigitCode>): [0-9]{3,3};
                                (?<PhoneNumber>): ThreeDigitCode '-' ThreeDigitCode '-' (?<FourDigitCode>[0-9]{4});
                              ";

			AExpression ROOT = PEGrammar.Load(grammar);

			String input = "123-456-7890";
			var iterator = new StringInputIterator(input);
			var visitor = new NpegParserVisitor(iterator);
			ROOT.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			AstNode node = visitor.AST;

			Assert.IsTrue(node.Token.Name == "PhoneNumber");
			Assert.IsTrue(node.Token.Value == input);
			Assert.IsTrue(node.Children[0].Token.Name == "ThreeDigitCode");
			Assert.IsTrue(node.Children[0].Token.Value == "123");
			Assert.IsTrue(node.Children[1].Token.Name == "ThreeDigitCode");
			Assert.IsTrue(node.Children[1].Token.Value == "456");
			Assert.IsTrue(node.Children[2].Token.Name == "FourDigitCode");
			Assert.IsTrue(node.Children[2].Token.Value == "7890");
		}


		[TestMethod]
		public void PEGrammar_PhoneNumber()
		{
			String input = "123-456-7890";

			AExpression PhoneNumber = PEGrammar.Load(
				@"
                        (?<ThreeDigitCode>): [0-9] [0-9] [0-9];
                        (?<FourDigitCode>): [0-9] [0-9] [0-9] [0-9];
                        (?<PhoneNumber>): ThreeDigitCode '-' ThreeDigitCode '-' FourDigitCode;
                    "
					.Trim());

			var visitor = new NpegParserVisitor(new StringInputIterator(input));
			PhoneNumber.Accept(visitor);

			Assert.IsTrue(visitor.IsMatch);
			AstNode node = visitor.AST;
			Assert.IsTrue(node.Token.Name == "PhoneNumber");
			Assert.IsTrue(node.Token.Value == input);
			Assert.IsTrue(node.Children[0].Token.Name == "ThreeDigitCode");
			Assert.IsTrue(node.Children[0].Token.Value == "123");
			Assert.IsTrue(node.Children[1].Token.Name == "ThreeDigitCode");
			Assert.IsTrue(node.Children[1].Token.Value == "456");
			Assert.IsTrue(node.Children[2].Token.Name == "FourDigitCode");
			Assert.IsTrue(node.Children[2].Token.Value == "7890");
		}

		[TestMethod]
		public void PEGrammar_RecursiveParentheses()
		{
			String input = "((((((123))))))";

			AExpression ROOT = PEGrammar.Load(
				@"
                        (?<DIGITS>): ([0-9])+;
                        (?<ENCLOSEDDIGITS>): '(' ParethesisFunction ')';
                        ParethesisFunction: (DIGITS / ENCLOSEDDIGITS);
                        (?<RECURSIONTEST>): ParethesisFunction;
                    "
					.Trim());

			var visitor = new NpegParserVisitor(new StringInputIterator(input));
			ROOT.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			AstNode node = visitor.AST;
			Assert.IsTrue(node.Token.Value == input);
		}

		[TestMethod]
		public void PEGrammar_MathematicalFormula_Recursion()
		{
			AExpression ROOT = PEGrammar.Load(
				@"
                    (?<Value>): [0-9]+ / '(' Expr ')';
                    (?<Product>): Value ((?<Symbol>'*' / '/') Value)*;
                    (?<Sum>): Product ((?<Symbol>'+' / '-') Product)*;
                    (?<Expr>): Sum;
                "
				);

			String input = "((((12/3)+5-2*(81/9))+1))";
			var iterator = new StringInputIterator(input);
			var visitor = new NpegParserVisitor(iterator);

			ROOT.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			AstNode node = visitor.AST;
			Assert.IsTrue(node.Token.Value == input);
#warning does not specify expected tree
		}


		[TestMethod]
		[ExpectedException(typeof (ParsingFatalTerminalException))]
		public void PEGrammar_Interpreter_Fatal()
		{
			AExpression ROOT = PEGrammar.Load(
				@"
                    (?<Value>): Fatal<'error'>;
                "
				);

			String input = " ";
			var iterator = new StringInputIterator(input);
			var visitor = new NpegParserVisitor(iterator);

			ROOT.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
		}


		[TestMethod]
		public void PEGrammar_Interpreter_Warn()
		{
			AExpression ROOT = PEGrammar.Load(
				@"
                    (?<Value>): Warn<'warning'>;
                "
				);

			String input = " ";
			var iterator = new StringInputIterator(input);
			var visitor = new NpegParserVisitor(iterator);

			ROOT.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			Assert.IsTrue(visitor.Warnings.Count == 1);
		}


		[TestMethod]
		public void PEGrammar_Interpreter_CodePoint()
		{
			AExpression ROOT = PEGrammar.Load(
				@"
                    (?<Value>): #x20;
                "
				);

			String input = " ";
			var iterator = new StringInputIterator(input);
			var visitor = new NpegParserVisitor(iterator);

			ROOT.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
		}


		[TestMethod]
		public void PEGrammar_BooleanAlgebra()
		{
			String grammar =
				@"
					S: [\s]+;
                    (?<Gate>): ('*' / 'AND') / ('~*' / 'NAND') / ('+' / 'OR') / ('~+' / 'NOR') / ('^' / 'XOR') / ('~^' / 'XNOR');
                    ValidVariable: '""' (?<Variable>[a-zA-Z0-9]+) '""'  / '\'' (?<Variable>[a-zA-Z0-9]+) '\'' / (?<Variable>[a-zA-Z]);
                    VarProjection1: ValidVariable /  (?<Invertor>'!' ValidVariable);
                    VarProjection2: VarProjection1 / '(' Expression ')' / (?<Invertor>'!' '(' Expression ')');
                    Expression: S? VarProjection2 S? (Gate S? VarProjection2 S?)*;
                    (?<BooleanEquation>): Expression !.;
                "
					.Trim();

			AExpression ROOT = PEGrammar.Load(grammar);

			// single variable
			var iterator = new StringInputIterator("A*!B+!A*B");
			var visitor = new NpegParserVisitor(iterator);
			ROOT.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			AstNode node = visitor.AST;
#warning Assert.IsTrue(node.Token.Value == input);


			// quoted variable
			iterator = new StringInputIterator("'aA'*!'bB'+!'aA'*'bB'");
			visitor = new NpegParserVisitor(iterator);
			ROOT.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			node = visitor.AST;
#warning             Assert.IsTrue(node.Token.Value == input);

			// expression + gate + variable .star()
			iterator = new StringInputIterator("A*!B*C+!A*B*C");
			visitor = new NpegParserVisitor(iterator);
			ROOT.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			node = visitor.AST;
#warning             Assert.IsTrue(node.Token.Value == input);

			// parethesis
			iterator = new StringInputIterator("((A)*(!B)+(!A)*(B))");
			visitor = new NpegParserVisitor(iterator);
			ROOT.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			node = visitor.AST;
#warning            Assert.IsTrue(node.Token.Value == input);

			iterator = new StringInputIterator("((A)*!(B)+!(A)*(B))");
			visitor = new NpegParserVisitor(iterator);
			ROOT.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			node = visitor.AST;
#warning            Assert.IsTrue(node.Token.Value == input);

			iterator = new StringInputIterator("((A)*(!(B))+(!(A))*(B))");
			visitor = new NpegParserVisitor(iterator);
			ROOT.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			node = visitor.AST;
#warning            Assert.IsTrue(node.Token.Value == input);

			iterator = new StringInputIterator("(!X*Y*!Z)");
			visitor = new NpegParserVisitor(iterator);
			ROOT.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			node = visitor.AST;
#warning             Assert.IsTrue(node.Token.Value == input);

			iterator = new StringInputIterator("(!X*Y*!Z)+(!X*Y*Z)");
			visitor = new NpegParserVisitor(iterator);
			ROOT.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			node = visitor.AST;
#warning             Assert.IsTrue(node.Token.Value == input);

			iterator = new StringInputIterator("(X*Z)");
			visitor = new NpegParserVisitor(iterator);
			ROOT.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			node = visitor.AST;
#warning             Assert.IsTrue(node.Token.Value == input);

			iterator = new StringInputIterator("(!X*Y*!Z)+(!X*Y*Z)+(X*Z)");
			visitor = new NpegParserVisitor(iterator);
			ROOT.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			node = visitor.AST;
#warning            Assert.IsTrue(node.Token.Value == input);

			iterator = new StringInputIterator("((((!X*Y*Z)+(!X*Y*!Z)+(X*Z))))");
			visitor = new NpegParserVisitor(iterator);
			ROOT.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			node = visitor.AST;
#warning            Assert.IsTrue(node.Token.Value == input);
		}
	}
}