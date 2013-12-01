using System.Text;
using NPEG.Extensions;
using NPEG.NonTerminals;
using NPEG.Terminals;
using NUnit.Framework;

namespace NPEG.Tests
{
	[TestFixture]
	public class CompositeVisitorTests
	{
		public TestContext TestContext { get; set; }


		[Test]
		public void CompositeVisitor_RecursiveParentheses()
		{
			#region Composite

			AExpression DIGITS = new CapturingGroup("DIGITS", new OneOrMore(new CharacterClass {ClassExpression = "[0-9]"}));
			AExpression ENCLOSEDDIGITS = new RecursionCreate("ParethesisFunction",
			                                                 new PrioritizedChoice(
			                                                 	DIGITS
			                                                 	,
			                                                 	new CapturingGroup("ENCLOSEDDIGITS",
			                                                 	                   new Sequence(
			                                                 	                   	new Literal {MatchText = "("},
			                                                 	                   	new RecursionCall("ParethesisFunction")
			                                                 	                   	).Sequence(new Literal {MatchText = ")"})
			                                                 		)
			                                                 	)
				);

			AExpression ROOT = new CapturingGroup("RECURSIONTEST", ENCLOSEDDIGITS);

			#endregion

			var input = Encoding.UTF8.GetBytes("((((((123))))))");
			var visitor = new NpegParserVisitor(new ByteInputIterator(input));
			ROOT.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			AstNode node = visitor.AST;
			Assert.IsTrue(node.Token.Name == "RECURSIONTEST");
			Assert.IsTrue(node.Children.Count == 1);
			Assert.IsTrue(node.Children[0].Token.Name == "ENCLOSEDDIGITS");
			Assert.IsTrue(node.Children[0].Children.Count == 1);
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "ENCLOSEDDIGITS");
			Assert.IsTrue(node.Children[0].Children[0].Children.Count == 1);
			Assert.IsTrue(node.Children[0].Children[0].Children[0].Token.Name == "ENCLOSEDDIGITS");
			Assert.IsTrue(node.Children[0].Children[0].Children.Count == 1);
			Assert.IsTrue(node.Children[0].Children[0].Children[0].Children[0].Token.Name == "ENCLOSEDDIGITS");
			Assert.IsTrue(node.Children[0].Children[0].Children[0].Children[0].Children.Count == 1);
			Assert.IsTrue(node.Children[0].Children[0].Children[0].Children[0].Children[0].Token.Name == "ENCLOSEDDIGITS");
			Assert.IsTrue(node.Children[0].Children[0].Children[0].Children[0].Children[0].Children.Count == 1);
			Assert.IsTrue(node.Children[0].Children[0].Children[0].Children[0].Children[0].Children[0].Token.Name == "ENCLOSEDDIGITS");
			Assert.IsTrue(node.Children[0].Children[0].Children[0].Children[0].Children[0].Children[0].Children.Count == 1);
			Assert.IsTrue(node.Children[0].Children[0].Children[0].Children[0].Children[0].Children[0].Children[0].Token.Name == "DIGITS");
			Assert.IsTrue(node.Children[0].Children[0].Children[0].Children[0].Children[0].Children[0].Children[0].Children.Count == 0);
		}


		[Test]
		public void CompositeVisitor_NestedRecursive()
		{
			#region Composite

			var DIGITS = new CapturingGroup("DIGITS", new OneOrMore(new CharacterClass {ClassExpression = "[0-9]"}));
			var LTENCLOSED = new RecursionCreate("RECURSIONLTENCLOSED",
			                                     new PrioritizedChoice(DIGITS,
			                                                           new CapturingGroup("LTENCLOSED",
			                                                                              new Sequence(
			                                                                              	new Literal {MatchText = "<"},
			                                                                              	new RecursionCall(
			                                                                              		"RECURSIONLTENCLOSED")
			                                                                              	).Sequence(new Literal
			                                                                              	           	{MatchText = ">"})
			                                                           	)
			                                     	)
				);
			var PENCLOSED = new RecursionCreate("RECURSIONPENCLOSED",
			                                    new PrioritizedChoice(LTENCLOSED,
			                                                          new CapturingGroup("PENCLOSED",
			                                                                             new Sequence(
			                                                                             	new Literal {MatchText = "("},
			                                                                             	new RecursionCall("RECURSIONPENCLOSED")
			                                                                             	).Sequence(new Literal
			                                                                             	           	{MatchText = ")"})
			                                                          	)
			                                    	)
				);

			AExpression ROOT = new CapturingGroup("NESTEDRECURSIONTEST", PENCLOSED);

			#endregion

			var input = "(((<<<123>>>)))";
			var bytes = Encoding.UTF8.GetBytes(input);
			var iterator = new ByteInputIterator(bytes);
			var visitor = new NpegParserVisitor(iterator);
			ROOT.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			AstNode node = visitor.AST;
			Assert.IsTrue(node.Token.ValueAsString(iterator) == input);
			Assert.IsTrue(node.Token.Name == "NESTEDRECURSIONTEST");
			Assert.IsTrue(node.Children.Count == 1);
			Assert.IsTrue(node.Children[0].Token.Name == "PENCLOSED");
			Assert.IsTrue(node.Children[0].Children.Count == 1);
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "PENCLOSED");
			Assert.IsTrue(node.Children[0].Children[0].Children.Count == 1);
			Assert.IsTrue(node.Children[0].Children[0].Children[0].Token.Name == "PENCLOSED");
			Assert.IsTrue(node.Children[0].Children[0].Children[0].Children.Count == 1);
			Assert.IsTrue(node.Children[0].Children[0].Children[0].Children[0].Token.Name == "LTENCLOSED");
			Assert.IsTrue(node.Children[0].Children[0].Children[0].Children[0].Children.Count == 1);
			Assert.IsTrue(node.Children[0].Children[0].Children[0].Children[0].Children[0].Token.Name == "LTENCLOSED");
			Assert.IsTrue(node.Children[0].Children[0].Children[0].Children[0].Children[0].Children.Count == 1);
			Assert.IsTrue(node.Children[0].Children[0].Children[0].Children[0].Children[0].Children[0].Token.Name == "LTENCLOSED");
			Assert.IsTrue(node.Children[0].Children[0].Children[0].Children[0].Children[0].Children[0].Children.Count == 1);
		}


		[Test]
		public void CompositeVisitor_CapturingGroup_SandBoxTest_PriorityChoice1()
		{
			PrioritizedChoice newline = new PrioritizedChoice(
				new Literal {MatchText = "\r\n"}, // windows
				new Literal {MatchText = "\r\r"} // old macs
				)
				.Or(new Literal {MatchText = "\n"}); // linux

			// Single Line Comment
			var singleLineComment = new Sequence(
				new Literal {MatchText = "//"},
				new Sequence(
					new NotPredicate(newline),
					new AnyCharacter()
					)
					.Star()
				);

			// Multiline Comment
			var multiLineComment = new Sequence(
				new Literal {MatchText = "/*"},
				new Sequence(
					new NotPredicate(new Literal {MatchText = "*/"}),
					new AnyCharacter()
					)
					.Star()
					.Sequence(new Literal {MatchText = "*/"})
				);

			var comment = new PrioritizedChoice(singleLineComment, multiLineComment);

			var whitespace = new PrioritizedChoice(
				new CharacterClass {ClassExpression = "[ \t\r\n\v]"},
				comment
				);

			var label = new CapturingGroup("Label",
			                               new Sequence(
			                               	new CharacterClass {ClassExpression = "[a-zA-Z_]"},
			                               	// must start with alpha character
			                               	new ZeroOrMore(new CharacterClass {ClassExpression = "[a-zA-Z0-9_]"})
			                               	)
				);

			var backreference = new CapturingGroup("DynamicBackReferencing",
			                                       new Sequence(
			                                       	new Literal {MatchText = @"\k<"},
			                                       	new Sequence(new ZeroOrMore(whitespace), label).Sequence(
			                                       		new ZeroOrMore(whitespace))
			                                       	)
			                                       	.Sequence(
			                                       		new Optional(
			                                       			new Sequence(
			                                       				new Sequence(
			                                       					new Literal {MatchText = "["},
			                                       					new CapturingGroup("CaseSensitive",
			                                       					                   new Literal {MatchText = @"\i"}
			                                       						)
			                                       					),
			                                       				new Literal {MatchText = "]"}
			                                       				)
			                                       			)
			                                       	)
			                                       	.Sequence(
			                                       		new Sequence(new ZeroOrMore(whitespace), new Literal {MatchText = ">"})
			                                       	)
				);

			var root = new CapturingGroup("Test",
			                              new Sequence(
			                              	backreference,
			                              	new NotPredicate(new AnyCharacter())
			                              	)
				);

			var input = @"\k< CapturedLabelVariableName >";
			var bytes = Encoding.UTF8.GetBytes(input);
			var iterator = new ByteInputIterator(bytes);
			var visitor = new NpegParserVisitor(iterator);

			root.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			AstNode node = visitor.AST;
			Assert.IsTrue(node.Token.Name == "Test");
			Assert.IsTrue(node.Children[0].Token.Name == "DynamicBackReferencing");
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "Label");
			Assert.IsTrue(node.Children[0].Children[0].Token.ValueAsString(iterator) == "CapturedLabelVariableName");
		}


		[Test]
		public void CompositeVisitor_CapturingGroup_SandBoxTest_PriorityChoice2()
		{
			var mSpace = new CharacterClass {ClassExpression = "[ \t]"};
			var limiting = new CapturingGroup("LimitingRepetition",
			                                  new Sequence(
			                                  	new Sequence(
			                                  		new Literal {MatchText = "{"},
			                                  		new ZeroOrMore(mSpace)
			                                  		),
			                                  	new PrioritizedChoice(
			                                  		new CapturingGroup("BETWEEN",
			                                  		                   new Sequence(
			                                  		                   	new CapturingGroup("Min",
			                                  		                   	                   new OneOrMore(new CharacterClass
			                                  		                   	                                 	{ClassExpression = "[0-9]"}))
			                                  		                   		.Sequence(new ZeroOrMore(mSpace)),
			                                  		                   	new Literal {MatchText = ","}
			                                  		                   	)
			                                  		                   	.Sequence(
			                                  		                   		new Sequence(new ZeroOrMore(mSpace),
			                                  		                   		             new CapturingGroup("Max",
			                                  		                   		                                new OneOrMore(
			                                  		                   		                                	new CharacterClass
			                                  		                   		                                		{
			                                  		                   		                                			ClassExpression = "[0-9]"
			                                  		                   		                                		})))
			                                  		                   	)
			                                  			)
			                                  		,
			                                  		new CapturingGroup("ATMOST",
			                                  		                   new Sequence(
			                                  		                   	new Literal {MatchText = ","}
			                                  		                   	,
			                                  		                   	new Sequence(new ZeroOrMore(mSpace),
			                                  		                   	             new CapturingGroup("Max",
			                                  		                   	                                new OneOrMore(
			                                  		                   	                                	new CharacterClass
			                                  		                   	                                		{ClassExpression = "[0-9]"})))
			                                  		                   	)
			                                  			)
			                                  		)
			                                  		.Or
			                                  		(
			                                  			new CapturingGroup("ATLEAST",
			                                  			                   new Sequence(
			                                  			                   	new Sequence(new ZeroOrMore(mSpace),
			                                  			                   	             new CapturingGroup("Min",
			                                  			                   	                                new OneOrMore(
			                                  			                   	                                	new CharacterClass
			                                  			                   	                                		{
			                                  			                   	                                			ClassExpression = "[0-9]"
			                                  			                   	                                		}))).Sequence(
			                                  			                   	                                			new ZeroOrMore(mSpace))
			                                  			                   	,
			                                  			                   	new Literal {MatchText = ","}
			                                  			                   	)
			                                  				)
			                                  		)
			                                  		.Or
			                                  		(
			                                  			new CapturingGroup("EXACT",
			                                  			                   new OneOrMore(new CharacterClass {ClassExpression = "[0-9]"}))
			                                  		)
			                                  	)
			                                  	.Sequence(
			                                  		new ZeroOrMore(mSpace)
			                                  	)
			                                  	.Sequence(
			                                  		new Literal {MatchText = "}"}
			                                  	)
				);

			var any = new CapturingGroup("AnyCharacter", new Literal {MatchText = "."});

			var expression = new CapturingGroup("Expression",
			                                    new PrioritizedChoice(
			                                    	new Sequence(any, limiting),
			                                    	new Sequence(limiting, any)
			                                    	)
				);

			var input = ".{77,55}";
			var bytes = Encoding.UTF8.GetBytes(input);
			var visitor = new NpegParserVisitor(
				new ByteInputIterator(bytes)
			);
			expression.Accept(visitor);

			Assert.IsTrue(visitor.IsMatch);
			AstNode node = visitor.AST;
			Assert.IsTrue(node.Token.Name == "Expression");
			Assert.IsTrue(node.Children.Count == 2);
			Assert.IsTrue(node.Children[0].Token.Name == "AnyCharacter");
			Assert.IsTrue(node.Children[1].Token.Name == "LimitingRepetition");
		}


		[Test]
		public void CompositeVisitor_CapturingGroup_SandBoxTest_PriorityChoice3()
		{
			var prefix = new PrioritizedChoice(
				new CapturingGroup("AndPredicate", new Literal {MatchText = "&"}),
				new CapturingGroup("NotPredicate", new Literal {MatchText = "!"})
				);

			PrioritizedChoice suffix = new PrioritizedChoice(
				new CapturingGroup("ZeroOrMore", new Literal {MatchText = "*"}),
				new CapturingGroup("OneOrMore", new Literal {MatchText = "+"})
				)
				.Or(new CapturingGroup("Optional", new Literal {MatchText = "?"}));

			var terminal = new CapturingGroup("AnyCharacter", new Literal {MatchText = "."});
			var expression = new CapturingGroup("Expression",
			                                    new PrioritizedChoice(
			                                    	// match prefixes first
			                                    	prefix.Plus()
			                                    		.Sequence(terminal)
			                                    	,
			                                    	// match suffixes next
			                                    	terminal
			                                    		.Sequence(
			                                    			suffix.Plus()
			                                    		)
			                                    	)
			                                    	.Or(terminal)
			                                    	.Plus()
				);

			var input = ".";
			var bytes = Encoding.UTF8.GetBytes(input);
			var iterator = new ByteInputIterator(bytes);
			var visitor = new NpegParserVisitor(iterator);
			expression.Accept(visitor);

			Assert.IsTrue(visitor.IsMatch);
			AstNode node = visitor.AST;
			Assert.IsTrue(node.Children.Count == 1);
			Assert.IsTrue(node.Token.Name == "Expression");
			Assert.IsTrue(node.Token.ValueAsString(iterator) == ".");
			Assert.IsTrue(node.Children[0].Token.Name == "AnyCharacter");
		}


		[Test]
		public void CompositeVisitor_Recursiveness()
		{
			var whitespace = new CharacterClass {ClassExpression = "[ \t\r\n\v]"};

			var terminal = new PrioritizedChoice(
				new CapturingGroup("AnyCharacter", new Literal {MatchText = "."})
				,
				new CapturingGroup("CapturingGroup",
				                   new Sequence(
				                   	new Literal {MatchText = "(?<"},
				                   	new CapturingGroup("ReplacementNode",
				                   	                   new OneOrMore(
				                   	                   	new CharacterClass {ClassExpression = "[a-z0-9A-Z]"}
				                   	                   	)
				                   		)
				                   	)
				                   	.Sequence(new Literal {MatchText = ">"})
				                   	.Sequence(new RecursionCall("Expression"))
				                   	.Sequence(new Literal {MatchText = ")"})
					)
				);

			var sequence = new CapturingGroup(
				"Sequence",
				new Sequence(
					terminal,
					new ZeroOrMore(whitespace)
					).Plus()
				) {DoReplaceBySingleChildNode = true};

			var prioritizedchoice = new CapturingGroup("PrioritizedChoice",
			                                           new Sequence(
			                                           	sequence,
			                                           	new Literal {MatchText = "/"}
			                                           	)
			                                           	.Sequence(new ZeroOrMore(whitespace))
			                                           	.Sequence(sequence)
			                                           	.Sequence(
			                                           		new ZeroOrMore(
			                                           			new Sequence(
			                                           				new ZeroOrMore(whitespace),
			                                           				new Literal {MatchText = "/"}
			                                           				)
			                                           				.Sequence(new ZeroOrMore(whitespace))
			                                           				.Sequence(sequence)
			                                           				.Plus()
			                                           			)
			                                           	)
				);

			var expression = new CapturingGroup("Root",
			                                    new RecursionCreate("Expression",
			                                                        new PrioritizedChoice(prioritizedchoice, sequence)));


			var input = @"(?<NPEGNode>./.. )";
			var bytes = Encoding.UTF8.GetBytes(input);
			var iterator = new ByteInputIterator(bytes);
			var visitor = new NpegParserVisitor(iterator);
			expression.Accept(visitor);

			Assert.IsTrue(visitor.IsMatch);
			AstNode node = visitor.AST;
			Assert.IsTrue(node.Token.Name == "Root");
			Assert.IsTrue(node.Children.Count == 1);
			Assert.IsTrue(node.Children.Count == 1);
			Assert.IsTrue(node.Children[0].Token.Name == "CapturingGroup");
			Assert.IsTrue(node.Children[0].Children.Count == 2);
			Assert.IsTrue(node.Children[0].Children[0].Token.Name == "ReplacementNode");
			Assert.IsTrue(node.Children[0].Children[1].Token.Name == "PrioritizedChoice");
			Assert.IsTrue(node.Children[0].Children[1].Children[0].Token.Name == "AnyCharacter");
			Assert.IsTrue(node.Children[0].Children[1].Children[1].Token.Name == "Sequence");
			Assert.IsTrue(node.Children[0].Children[1].Children[1].Children[0].Token.Name == "AnyCharacter");
			Assert.IsTrue(node.Children[0].Children[1].Children[1].Children[1].Token.Name == "AnyCharacter");
		}
	}
}