using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NPEG.Algorithms.LanguageWriters;
using NPEG.GrammarInterpreter;
using NPEG.NonTerminals;
using NPEG.Terminals;

namespace NPEG.Tests
{
	[TestClass]
	public class LanguageWriterTests
	{
		public TestContext TestContext { get; set; }


		[TestMethod]
		public void LanguageWriter_XML_DynamicBackReference()
		{
			#region Composite

			AExpression TAG = new CapturingGroup("TAG",
			                                     new OneOrMore(
			                                     	new CharacterClass {ClassExpression = "[a-zA-Z0-9]"}
			                                     	)
				);

			AExpression StartTag = new CapturingGroup("START_TAG",
			                                          new Sequence(
			                                          	new Literal {MatchText = "<"}, TAG)
			                                          	.Sequence(
			                                          		new Literal {MatchText = ">"}
			                                          	)
				);

			AExpression EndTag = new CapturingGroup("END_TAG",
			                                        new Sequence(
			                                        	new Literal {MatchText = "</"},
			                                        	new DynamicBackReference
			                                        		{
			                                        			BackReferenceName = "TAG",
			                                        			IsCaseSensitive = true
			                                        		}
			                                        	)
			                                        	.Sequence(
			                                        		new Literal {MatchText = ">"}
			                                        	)
				);


			AExpression Body = new CapturingGroup("Body", new Sequence(new NotPredicate(EndTag), new AnyCharacter()).Star());

			AExpression Expression = new CapturingGroup("Expression", new Sequence(StartTag, Body).Sequence(EndTag).Plus());

			#endregion

			String input = "<h1>hello</h1><h2>hello</h2>";

			var cpluspluswriter = new CPlusPlusWriter("SimpleXml", "./SimpleXml/CPlusPlus");
			Expression.Accept(cpluspluswriter);
			cpluspluswriter.Write();

			var cwriter = new CWriter("SimpleXml", "./SimpleXml/C");
			Expression.Accept(cwriter);
			cwriter.Write();

			var csharpwriter = new CSharpWriter("SimpleXml", "RobustHaven.Text.Npeg.Tests.Parsers", "./SimpleXml/CSharp");
			Expression.Accept(csharpwriter);
			csharpwriter.Write();

			var javawriter = new JavaWriter("SimpleXml", "RobustHaven.Text.Npeg.Tests.Parsers".ToLower(), "./SimpleXml/Java");
			Expression.Accept(javawriter);
			javawriter.Write();

			var parser = new NpegParserVisitor(
				new StringInputIterator(input));
			Expression.Accept(parser);
			Assert.IsTrue(parser.IsMatch);
			AstNode node = parser.AST;
			Assert.IsTrue(node.Token.Name == "Expression");
			Assert.IsTrue(node.Children.Count == 6);

			Assert.IsTrue(node.Children[0].Token.Name == "START_TAG");
			Assert.IsTrue(node.Children[0].Children.Count == 1);
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "TAG");
#warning Assert.IsTrue(node.Children[0].Children[0].Children[0].Children.Count == 0);

			Assert.IsTrue(node.Children[1].Token.Name == "Body");
			Assert.IsTrue(node.Children[1].Children.Count == 0);

			Assert.IsTrue(node.Children[2].Token.Name == "END_TAG");
#warning Assert.IsTrue(node.Children[2].Children.Count == 1);
#warning Assert.IsTrue(node.Children[2].Children[0].Token.Name == "TAG");
#warning Assert.IsTrue(node.Children[2].Children[0].Children[0].Children.Count == 0);


			Assert.IsTrue(node.Children[3].Token.Name == "START_TAG");
			Assert.IsTrue(node.Children[3].Children.Count == 1);
			Assert.IsTrue(node.Children[3].Children[0].Token.Name == "TAG");
#warning Assert.IsTrue(node.Children[3].Children[0].Children[0].Children.Count == 0);

			Assert.IsTrue(node.Children[4].Token.Name == "Body");
			Assert.IsTrue(node.Children[4].Children.Count == 0);

			Assert.IsTrue(node.Children[5].Token.Name == "END_TAG");
#warning Assert.IsTrue(node.Children[5].Children.Count == 1);
#warning Assert.IsTrue(node.Children[5].Children[0].Token.Name == "TAG");
#warning Assert.IsTrue(node.Children[5].Children[0].Children[0].Children.Count == 0);
		}


		[TestMethod]
		public void LanguageWriter_MathematicalFormula_RecursionCall()
		{
			String grammar =
				@"
                    (?<Value>): [0-9]+ / '(' Expr ')';
                    (?<Product>): Value ((?<Symbol>'*' / '/') Value)*;
                    (?<Sum>): Product ((?<Symbol>'+' / '-') Product)*;
                    (?<Expr>): Sum;
            ";

			AExpression Expression = PEGrammar.Load(grammar);

			String input = "((((12/3)+5-2*(81/9))+1))";

			var cpluspluswriter = new CPlusPlusWriter("MathematicalFormula", "./MathematicalFormula/CPlusPlus");
			Expression.Accept(cpluspluswriter);
			cpluspluswriter.Write();

			var cwriter = new CWriter("MathematicalFormula", "./MathematicalFormula/C");
			Expression.Accept(cwriter);
			cwriter.Write();

			var csharpwriter = new CSharpWriter("MathematicalFormula", "RobustHaven.Text.Npeg.Tests.Parsers",
			                                    "./MathematicalFormula/CSharp");
			Expression.Accept(csharpwriter);
			csharpwriter.Write();

			var javawriter = new JavaWriter("MathematicalFormula", "RobustHaven.Text.Npeg.Tests.Parsers".ToLower(),
			                                "./MathematicalFormula/Java");
			Expression.Accept(javawriter);
			javawriter.Write();


#warning need complete composite
		}


		[TestMethod]
		public void LanguageWriter_PhoneNumber_CapuringGroup_Sequence()
		{
			String grammar =
				@"
                                (?<ThreeDigitCode>): [0-9]{3,3};
                                (?<PhoneNumber>): ThreeDigitCode '-' ThreeDigitCode '-' (?<FourDigitCode>[0-9]{4});
                              ";

			AExpression PhoneNumber = PEGrammar.Load(grammar);

			var cpluspluswriter = new CPlusPlusWriter("PhoneNumber", "./PhoneNumber/CPlusPlus");
			PhoneNumber.Accept(cpluspluswriter);
			cpluspluswriter.Write();

			var cwriter = new CWriter("PhoneNumber", "./PhoneNumber/C");
			PhoneNumber.Accept(cwriter);
			cwriter.Write();

			var csharpwriter = new CSharpWriter("PhoneNumber", "RobustHaven.Text.Npeg.Tests.Parsers", "./PhoneNumber/CSharp");
			PhoneNumber.Accept(csharpwriter);
			csharpwriter.Write();

			var javawriter = new JavaWriter("PhoneNumber", "RobustHaven.Text.Npeg.Tests.Parsers".ToLower(), "./PhoneNumber/Java");
			PhoneNumber.Accept(javawriter);
			javawriter.Write();

#warning need complete composite
			//var input = new IteratorForString(@"NpEg");
			//var visitor = new CompositeMatchVisitor(input);
			//NpegNode.Accept(visitor);
			//Assert.IsTrue(visitor.IsMatch);
			//var node = visitor.AST;
			//Assert.IsTrue(node.Token.Name == "NpegNode");
			//Assert.IsTrue(node.Token.Value == "NpEg");
		}

		[TestMethod]
		public void LanguageWriter_NpegNode_CapturingChoice_PrioritizedChoice()
		{
			AExpression NpegNode = new CapturingGroup("NpegNode",
			                                          new PrioritizedChoice(
			                                          	new Literal {MatchText = "NPEG", IsCaseSensitive = false},
			                                          	new Literal
			                                          		{MatchText = ".NET Parsing Expression Grammar", IsCaseSensitive = false}));

			var cpluspluswriter = new CPlusPlusWriter("NpegNode", "./NpegNode/CPlusPlus");
			NpegNode.Accept(cpluspluswriter);
			cpluspluswriter.Write();

			var cwriter = new CWriter("NpegNode", "./NpegNode/C");
			NpegNode.Accept(cwriter);
			cwriter.Write();

			var csharpwriter = new CSharpWriter("NpegNode", "RobustHaven.Text.Npeg.Tests.Parsers", "./NpegNode/CSharp");
			NpegNode.Accept(csharpwriter);
			csharpwriter.Write();

			var javawriter = new JavaWriter("NpegNode", "RobustHaven.Text.Npeg.Tests.Parsers".ToLower(), "./NpegNode/Java");
			NpegNode.Accept(javawriter);
			javawriter.Write();


			var input = new StringInputIterator(@"NpEg");
			var visitor = new NpegParserVisitor(input);
			NpegNode.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			AstNode node = visitor.AST;
			Assert.IsTrue(node.Token.Name == "NpegNode");
			Assert.IsTrue(node.Token.Value == "NpEg");


			input = new StringInputIterator(@".NET Parsing Expression Grammar");
			visitor = new NpegParserVisitor(input);
			NpegNode.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			node = visitor.AST;
			Assert.IsTrue(node.Token.Name == "NpegNode");
			Assert.IsTrue(node.Token.Value == ".NET Parsing Expression Grammar");


			input = new StringInputIterator(@"NET Parsing Expression Grammar");
			visitor = new NpegParserVisitor(input);
			NpegNode.Accept(visitor);
			Assert.IsFalse(visitor.IsMatch);
		}


		[TestMethod]
		public void LanguageWriter_InfiniteLoopTests_ZeroOrMore()
		{
			AExpression zeroormore = PEGrammar.Load(@"(?<Expression>): (.*)*;");

			var cpluspluswriter = new CPlusPlusWriter("ZeroOrMoreInfiniteLoopTest", "./ZeroOrMoreInfiniteLoopTest/CPlusPlus");
			zeroormore.Accept(cpluspluswriter);
			cpluspluswriter.Write();

			var cwriter = new CWriter("ZeroOrMoreInfiniteLoopTest", "./ZeroOrMoreInfiniteLoopTest/C");
			zeroormore.Accept(cwriter);
			cwriter.Write();

			var csharpwriter = new CSharpWriter("ZeroOrMoreInfiniteLoopTest", "RobustHaven.Text.Npeg.Tests.Parsers",
			                                    "./ZeroOrMoreInfiniteLoopTest/CSharp");
			zeroormore.Accept(csharpwriter);
			csharpwriter.Write();

			var javawriter = new JavaWriter("ZeroOrMoreInfiniteLoopTest", "RobustHaven.Text.Npeg.Tests.Parsers".ToLower(),
			                                "./ZeroOrMoreInfiniteLoopTest/Java");
			zeroormore.Accept(javawriter);
			javawriter.Write();
		}

		[TestMethod]
		public void LanguageWriter_InfiniteLoopTests_OneOrMore()
		{
			AExpression oneormore = PEGrammar.Load(@"(?<Expression>): (.*)+;");

			var cpluspluswriter = new CPlusPlusWriter("OneOrMoreInfiniteLoopTest", "./OneOrMoreInfiniteLoopTest/CPlusPlus");
			oneormore.Accept(cpluspluswriter);
			cpluspluswriter.Write();

			var cwriter = new CWriter("OneOrMoreInfiniteLoopTest", "./OneOrMoreInfiniteLoopTest/C");
			oneormore.Accept(cwriter);
			cwriter.Write();

			var csharpwriter = new CSharpWriter("OneOrMoreInfiniteLoopTest", "RobustHaven.Text.Npeg.Tests.Parsers",
			                                    "./OneOrMoreInfiniteLoopTest/CSharp");
			oneormore.Accept(csharpwriter);
			csharpwriter.Write();

			var javawriter = new JavaWriter("OneOrMoreInfiniteLoopTest", "RobustHaven.Text.Npeg.Tests.Parsers".ToLower(),
			                                "./OneOrMoreInfiniteLoopTest/Java");
			oneormore.Accept(javawriter);
			javawriter.Write();
		}

		[TestMethod]
		public void LanguageWriter_InfiniteLoopTests_LimitingRepetition()
		{
			AExpression limitingrepetition = PEGrammar.Load(@"(?<Expression>): (.*){0,};");

			var cpluspluswriter = new CPlusPlusWriter("LimitingRepetitionInfiniteLoopTest",
			                                          "./LimitingRepetitionInfiniteLoopTest/CPlusPlus");
			limitingrepetition.Accept(cpluspluswriter);
			cpluspluswriter.Write();

			var cwriter = new CWriter("LimitingRepetitionInfiniteLoopTest", "./LimitingRepetitionInfiniteLoopTest/C");
			limitingrepetition.Accept(cwriter);
			cwriter.Write();

			var csharpwriter = new CSharpWriter("LimitingRepetitionInfiniteLoopTest", "RobustHaven.Text.Npeg.Tests.Parsers",
			                                    "./LimitingRepetitionInfiniteLoopTest/CSharp");
			limitingrepetition.Accept(csharpwriter);
			csharpwriter.Write();

			var javawriter = new JavaWriter("LimitingRepetitionInfiniteLoopTest", "RobustHaven.Text.Npeg.Tests.Parsers".ToLower(),
			                                "./LimitingRepetitionInfiniteLoopTest/Java");
			limitingrepetition.Accept(javawriter);
			javawriter.Write();
		}

		[TestMethod]
		public void LanguageWriter_Simple_AndPredicateTest()
		{
			AExpression andpredicate = PEGrammar.Load(@"(?<Expression>): &'literal' 'literal';");

			var cpluspluswriter = new CPlusPlusWriter("SimpleAndPredicateTest", "./SimpleAndPredicateTest/CPlusPlus");
			andpredicate.Accept(cpluspluswriter);
			cpluspluswriter.Write();

			var cwriter = new CWriter("SimpleAndPredicateTest", "./SimpleAndPredicateTest/C");
			andpredicate.Accept(cwriter);
			cwriter.Write();

			var csharpwriter = new CSharpWriter("SimpleAndPredicateTest", "RobustHaven.Text.Npeg.Tests.Parsers",
			                                    "./SimpleAndPredicateTest/CSharp");
			andpredicate.Accept(csharpwriter);
			csharpwriter.Write();

			var javawriter = new JavaWriter("SimpleAndPredicateTest", "RobustHaven.Text.Npeg.Tests.Parsers".ToLower(),
			                                "./SimpleAndPredicateTest/Java");
			andpredicate.Accept(javawriter);
			javawriter.Write();
		}


		[TestMethod]
		public void LanguageWriter_Simple_CapturingGroupTest()
		{
			AExpression simplecapturegroup = PEGrammar.Load(@"(?<Expression>): 'literal';");

			var cpluspluswriter = new CPlusPlusWriter("SimpleCapturingGroupTest", "./SimpleCapturingGroupTest/CPlusPlus");
			simplecapturegroup.Accept(cpluspluswriter);
			cpluspluswriter.Write();

			var cwriter = new CWriter("SimpleCapturingGroupTest", "./SimpleCapturingGroupTest/C");
			simplecapturegroup.Accept(cwriter);
			cwriter.Write();

			var csharpwriter = new CSharpWriter("SimpleCapturingGroupTest", "RobustHaven.Text.Npeg.Tests.Parsers",
			                                    "./SimpleCapturingGroupTest/CSharp");
			simplecapturegroup.Accept(csharpwriter);
			csharpwriter.Write();

			var javawriter = new JavaWriter("SimpleCapturingGroupTest", "RobustHaven.Text.Npeg.Tests.Parsers".ToLower(),
			                                "./SimpleCapturingGroupTest/Java");
			simplecapturegroup.Accept(javawriter);
			javawriter.Write();
		}

		[TestMethod]
		public void LanguageWriter_RSC_CapturingGroupTest()
		{
			//replace single child node, note \rsc in parent will be ignored.
			AExpression capturegrouprsc = PEGrammar.Load(@"(?<Expression \rsc>): (?<ExpressionInner> 'literal');");
			//new CapturingGroup("Expression", new CapturingGroup("ExpressionInner", new Literal() { MatchText = "literal" })) { DoReplaceBySingleChildNode = true };
			var input = new StringInputIterator(@"literal");
			var visitor = new NpegParserVisitor(input);
			capturegrouprsc.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			AstNode node = visitor.AST;
			Assert.IsTrue(node.Children.Count == 0);
			Assert.IsTrue(node.Token.Name == "ExpressionInner");
			Assert.IsTrue(node.Token.Value == "literal");

			var cpluspluswriter = new CPlusPlusWriter("CapturingGroupRSCTest", "./CapturingGroupRSCTest/CPlusPlus");
			capturegrouprsc.Accept(cpluspluswriter);
			cpluspluswriter.Write();

			var cwriter = new CWriter("CapturingGroupRSCTest", "./CapturingGroupRSCTest/C");
			capturegrouprsc.Accept(cwriter);
			cwriter.Write();

			var csharpwriter = new CSharpWriter("CapturingGroupRSCTest", "RobustHaven.Text.Npeg.Tests.Parsers",
			                                    "./CapturingGroupRSCTest/CSharp");
			capturegrouprsc.Accept(csharpwriter);
			csharpwriter.Write();

			var javawriter = new JavaWriter("CapturingGroupRSCTest", "RobustHaven.Text.Npeg.Tests.Parsers".ToLower(),
			                                "./CapturingGroupRSCTest/Java");
			capturegrouprsc.Accept(javawriter);
			javawriter.Write();
		}
	}
}