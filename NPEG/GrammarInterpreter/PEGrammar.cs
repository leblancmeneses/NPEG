using System;
using System.Text;
using NPEG.ApplicationExceptions;
using NPEG.GrammarInterpreter.AstNodes;
using NPEG.NonTerminals;
using NPEG.Terminals;

namespace NPEG.GrammarInterpreter
{
	public class PEGrammar
	{
		private static AExpression mSpace()
		{
			return new CharacterClass {ClassExpression = "[ \t]"};
		}

		private static AExpression mNewLine()
		{
			return new PrioritizedChoice(
				new Literal {MatchText = "\r\n"}, // windows
				new Literal {MatchText = "\r\r"} // old macs
				)
				.Or(new Literal {MatchText = "\n"}); // linux
		}

		private static AExpression mComment()
		{
			// Single Line Comment
			AExpression singleLineComment = new Sequence(
				new Literal {MatchText = "//"},
				new Sequence(
					new NotPredicate(mNewLine()),
					new AnyCharacter()
					)
					.Star()
				);

			// Multiline Comment
			AExpression multiLineComment = new Sequence(
				new Literal {MatchText = "/*"},
				new Sequence(
					new NotPredicate(new Literal {MatchText = "*/"}),
					new AnyCharacter()
					)
					.Star()
					.Sequence(new Literal {MatchText = "*/"})
				);

			return new PrioritizedChoice(singleLineComment, multiLineComment);
		}

		private static AExpression mWhiteSpace()
		{
			return new PrioritizedChoice(
				new CharacterClass {ClassExpression = "[ \t\r\n\v]"},
				mComment()
				);
		}

		private static AExpression mHex()
		{
			return new Sequence(
				new Optional(new Literal {MatchText = "0"}),
				new CapturingGroup("Hex",
				                   new Sequence(
				                   	new Literal {MatchText = "x"},
				                   	new OneOrMore(new CharacterClass {ClassExpression = "[0-9A-Fa-f]"})
				                   	)
					)
				);
		}

		private static AExpression mBinary()
		{
			return new Sequence(
				new Optional(new Literal {MatchText = "0"}),
				new CapturingGroup("Binary",
				                   new Sequence(
				                   	new Literal {MatchText = "b"},
				                   	new OneOrMore(new CharacterClass {ClassExpression = "[0-1]"})
				                   	)
					)
				);
		}


		private static AExpression mLabel()
		{
			return new CapturingGroup("Label",
			                          new Sequence(
			                          	new CharacterClass {ClassExpression = "[a-zA-Z_]"}, // must start with alpha character
			                          	new ZeroOrMore(new CharacterClass {ClassExpression = "[a-zA-Z0-9_]"})
			                          	)
				);
		}

		private static AExpression mAnyCharacter()
		{
			return new CapturingGroup("AnyCharacter", new Literal {MatchText = "."});
		}

		private static AExpression mCharacterClass()
		{
			return new CapturingGroup("CharacterClass",
			                          new Sequence(
			                          	new Literal {MatchText = "["},
			                          	new OneOrMore(new CharacterClass {ClassExpression = @"[^\]]"})
			                          	)
			                          	.Sequence(new Literal {MatchText = "]"})
				);
		}

		private static AExpression mCodePoint()
		{
			return new CapturingGroup("CodePoint",
			                          new Sequence(
			                          	new Literal {MatchText = @"#"},
			                          	new PrioritizedChoice(
			                          		mHex(),
			                          		mBinary()
			                          		)
			                          		.Or(
			                          			new CapturingGroup("Decimal",
			                          			                   new OneOrMore(
			                          			                   	new CharacterClass {ClassExpression = "[0-9]"}
			                          			                   	)
			                          				)
			                          		)
			                          	)
				);
		}


		private static AExpression mDynamicBackReference()
		{
			return new CapturingGroup("DynamicBackReferencing",
			                          new Sequence(
			                          	new Literal {MatchText = @"\k<"},
			                          	new Sequence(new ZeroOrMore(mWhiteSpace()), mLabel()).Sequence(
			                          		new ZeroOrMore(mWhiteSpace()))
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
			                          		new Sequence(new ZeroOrMore(mWhiteSpace()), new Literal {MatchText = ">"})
			                          	)
				);
		}

		private static AExpression mFATAL()
		{
			return new CapturingGroup("Fatal",
			                          new Sequence(
			                          	new Literal {MatchText = "Fatal", IsCaseSensitive = false},
			                          	new Sequence(
			                          		new Sequence(new Literal {MatchText = "<"}, new Optional(mSpace())),
			                          		new PrioritizedChoice(
			                          			// with single quotes
			                          			new Sequence(
			                          				new Literal {MatchText = "'"},
			                          				new CapturingGroup("Message",
			                          				                   new OneOrMore(
			                          				                   	new PrioritizedChoice(
			                          				                   		new Sequence(
			                          				                   			new AndPredicate(new Literal {MatchText = @"\'"}),
			                          				                   			new LimitingRepetition(new AnyCharacter()) {Min = 2, Max = 2}
			                          				                   			),
			                          				                   		new CharacterClass {ClassExpression = @"[^']"}
			                          				                   		)
			                          				                   	)
			                          					)
			                          				)
			                          				.Sequence(new Literal {MatchText = "'"})
			                          			,
			                          			// without double quotes
			                          			new Sequence(
			                          				new Literal {MatchText = "\""},
			                          				new CapturingGroup("Message",
			                          				                   new OneOrMore(
			                          				                   	new PrioritizedChoice(
			                          				                   		new Sequence(
			                          				                   			new AndPredicate(new Literal {MatchText = @"\"""}),
			                          				                   			new LimitingRepetition(new AnyCharacter()) {Min = 2, Max = 2}
			                          				                   			),
			                          				                   		new CharacterClass {ClassExpression = @"[^""]"}
			                          				                   		)
			                          				                   	)
			                          					)
			                          				)
			                          				.Sequence(new Literal {MatchText = "\""})
			                          			)
			                          			.Or(
			                          				// no quotes
			                          			new Sequence(
			                          				new Optional(mSpace()),
			                          				new CapturingGroup("Message",
			                          				                   new OneOrMore(
			                          				                   	new Sequence(
			                          				                   		new NotPredicate(
			                          				                   			new Sequence(new Optional(mSpace()),
			                          				                   			             new Literal {MatchText = ">"})
			                          				                   			),
			                          				                   		new CharacterClass {ClassExpression = @"[^>]"}
			                          				                   		)
			                          				                   	)
			                          					)
			                          				)
			                          			)
			                          		)
			                          	)
			                          	.Sequence(
			                          		new Sequence(new Optional(mSpace()), new Literal {MatchText = ">"})
			                          	)
				);
		}

		private static AExpression mWARN()
		{
			return new CapturingGroup("Warn",
			                          new Sequence(
			                          	new Literal {MatchText = "Warn", IsCaseSensitive = false},
			                          	new Sequence(
			                          		new Sequence(new Literal {MatchText = "<"}, new Optional(mSpace())),
			                          		new PrioritizedChoice(
			                          			// with single quotes
			                          			new Sequence(
			                          				new Literal {MatchText = "'"},
			                          				new CapturingGroup("Message",
			                          				                   new OneOrMore(
			                          				                   	new PrioritizedChoice(
			                          				                   		new Sequence(
			                          				                   			new AndPredicate(new Literal {MatchText = @"\'"}),
			                          				                   			//new LimitingRepetition(new AnyCharacter()) { Min = 2, Max = 2 }
			                          				                   			new Sequence(new AnyCharacter(), new AnyCharacter())
			                          				                   			),
			                          				                   		new CharacterClass {ClassExpression = @"[^']"}
			                          				                   		)
			                          				                   	)
			                          					)
			                          				)
			                          				.Sequence(new Literal {MatchText = "'"})
			                          			,
			                          			// without double quotes
			                          			new Sequence(
			                          				new Literal {MatchText = "\""},
			                          				new CapturingGroup("Message",
			                          				                   new OneOrMore(
			                          				                   	new PrioritizedChoice(
			                          				                   		new Sequence(
			                          				                   			new AndPredicate(new Literal {MatchText = @"\"""}),
			                          				                   			//new LimitingRepetition(new AnyCharacter()) { Min = 2, Max = 2 }
			                          				                   			new Sequence(new AnyCharacter(), new AnyCharacter())
			                          				                   			),
			                          				                   		new CharacterClass {ClassExpression = @"[^""]"}
			                          				                   		)
			                          				                   	)
			                          					)
			                          				)
			                          				.Sequence(new Literal {MatchText = "\""})
			                          			)
			                          			.Or(
			                          				// no quotes
			                          			new Sequence(
			                          				new Optional(mSpace()),
			                          				new CapturingGroup("Message",
			                          				                   new OneOrMore(
			                          				                   	new Sequence(
			                          				                   		new NotPredicate(
			                          				                   			new Sequence(new Optional(mSpace()),
			                          				                   			             new Literal {MatchText = ">"})
			                          				                   			),
			                          				                   		new CharacterClass {ClassExpression = @"[^>]"}
			                          				                   		)
			                          				                   	)
			                          					)
			                          				)
			                          			)
			                          		)
			                          	)
			                          	.Sequence(
			                          		new Sequence(new Optional(mSpace()), new Literal {MatchText = ">"})
			                          	)
				);
		}

		private static AExpression mLiteral()
		{
			return new CapturingGroup("Literal",
			                          new Sequence(
			                          	new PrioritizedChoice(
			                          		new Sequence(
			                          			new Literal {MatchText = "\""},
			                          			new CapturingGroup("MatchText",
			                          			                   new OneOrMore(
			                          			                   	new PrioritizedChoice(
			                          			                   		new Sequence(
			                          			                   			new AndPredicate(
			                          			                   				new Literal {MatchText = @"\\"}
			                          			                   				)
			                          			                   			,
			                          			                   			// consumes two characters \" until it finds " not escaped
			                          			                   			new LimitingRepetition(new AnyCharacter()) {Min = 2, Max = 2}
			                          			                   			)
			                          			                   		,
			                          			                   		new PrioritizedChoice(
			                          			                   			new Sequence(
			                          			                   				new AndPredicate(
			                          			                   					new Literal {MatchText = @"\"""}
			                          			                   					)
			                          			                   				,
			                          			                   				// consumes two characters \" until it finds " not escaped
			                          			                   				new LimitingRepetition(new AnyCharacter()) {Min = 2, Max = 2}
			                          			                   				)
			                          			                   			,
			                          			                   			new CharacterClass {ClassExpression = "[^\"]"}
			                          			                   			)
			                          			                   		)
			                          			                   	)
			                          				)
			                          			)
			                          			.Sequence(new Literal {MatchText = "\""})
			                          		,
			                          		new Sequence(
			                          			new Literal {MatchText = "'"},
			                          			new CapturingGroup("MatchText",
			                          			                   new OneOrMore(
			                          			                   	new PrioritizedChoice(
			                          			                   		new Sequence(
			                          			                   			new AndPredicate(
			                          			                   				new Literal {MatchText = @"\\"}
			                          			                   				)
			                          			                   			,
			                          			                   			// consumes two characters \" until it finds " not escaped
			                          			                   			new LimitingRepetition(new AnyCharacter()) {Min = 2, Max = 2}
			                          			                   			)
			                          			                   		,
			                          			                   		new PrioritizedChoice(
			                          			                   			new Sequence(
			                          			                   				new AndPredicate(
			                          			                   					new Literal {MatchText = @"\'"}
			                          			                   					)
			                          			                   				,
			                          			                   				// consumes to characters \' until it finds ' not escaped
			                          			                   				new LimitingRepetition(new AnyCharacter()) {Min = 2, Max = 2}
			                          			                   				)
			                          			                   			,
			                          			                   			new CharacterClass {ClassExpression = "[^']"}
			                          			                   			)
			                          			                   		)
			                          			                   	)
			                          				)
			                          			)
			                          			.Sequence(new Literal {MatchText = "'"})
			                          		)
			                          	,
			                          	// optional do case-insensitive pattern matching. 
			                          	new Optional(new CapturingGroup("CaseInsensitive", new Literal {MatchText = @"\i"}))
			                          	)
				);
		}

		private static AExpression mRecursionCall()
		{
			return new CapturingGroup("RecursionCall", mLabel());
		}


		private static AExpression mCapturingGroup()
		{
			AExpression CG_ReplacementNode = new CapturingGroup("ReplacementNode", new Literal {MatchText = @"\rn"});

			AExpression CG_ReplaceBySingleChild = new CapturingGroup("ReplaceBySingleChild", new Literal {MatchText = @"\rsc"});

			AExpression CG_Options = new CapturingGroup("OptionalFlags",
			                                            new OneOrMore(
			                                            	new Sequence(
			                                            		new ZeroOrMore(mWhiteSpace())
			                                            		,
			                                            		new PrioritizedChoice(
			                                            			CG_ReplaceBySingleChild
			                                            			,
			                                            			CG_ReplacementNode
			                                            			)
			                                            		)
			                                            		.Sequence(new ZeroOrMore(mWhiteSpace()))
			                                            	)
				);

			return new CapturingGroup("CapturingGroup",
			                          new Sequence(
			                          	new Literal {MatchText = "("},
			                          	new Literal {MatchText = "?"}
			                          	)
			                          	.Sequence(new Literal {MatchText = "<"})
			                          	.Sequence(new ZeroOrMore(mSpace()))
			                          	.Sequence(mLabel())
			                          	.Sequence(new Optional(CG_Options))
			                          	.Sequence(new Sequence(new ZeroOrMore(mSpace()), new Literal {MatchText = ">"}))
			                          	.Sequence(new ZeroOrMore(mWhiteSpace()))
			                          	.Sequence(new RecursionCall("ExpressionRoot"))
			                          	.Sequence(new ZeroOrMore(mWhiteSpace()))
			                          	.Sequence(new Literal {MatchText = ")"})
				);
		}

		private static AExpression mTerminalReference()
		{
			AExpression CG_ReplacementNode = new CapturingGroup("ReplacementNode", new Literal {MatchText = @"\rn"});
			AExpression CG_ReplaceBySingleChild = new CapturingGroup("ReplaceBySingleChild", new Literal {MatchText = @"\rsc"});

			AExpression CG_Options = new CapturingGroup("OptionalFlags",
			                                            new OneOrMore(
			                                            	new Sequence(
			                                            		new ZeroOrMore(mWhiteSpace())
			                                            		,
			                                            		new PrioritizedChoice(
			                                            			CG_ReplaceBySingleChild
			                                            			,
			                                            			CG_ReplacementNode
			                                            			)
			                                            		)
			                                            		.Sequence(new ZeroOrMore(mWhiteSpace()))
			                                            	)
				);

			return new CapturingGroup("TerminalReference",
			                          new Sequence(
			                          	new Literal {MatchText = "("},
			                          	new Literal {MatchText = "?"}
			                          	)
			                          	.Sequence(new Literal {MatchText = "<"})
			                          	.Sequence(new ZeroOrMore(mSpace()))
			                          	.Sequence(mLabel())
			                          	.Sequence(new Optional(CG_Options))
			                          	.Sequence(new Sequence(new ZeroOrMore(mSpace()), new Literal {MatchText = ">"}))
			                          	.Sequence(new ZeroOrMore(mWhiteSpace()))
			                          	.Sequence(new Literal {MatchText = ")"})
				);
		}

		private static AExpression mGroup()
		{
			return new CapturingGroup("Group",
			                          new Sequence(
			                          	new Sequence(
			                          		new Literal {MatchText = "("}
			                          		,
			                          		new ZeroOrMore(mWhiteSpace())
			                          		)
			                          	,
			                          	new RecursionCall("ExpressionRoot")
			                          	)
			                          	.Sequence(new ZeroOrMore(mWhiteSpace()))
			                          	.Sequence(new Literal {MatchText = ")"})
				);
		}

		private static AExpression mTerminal()
		{
			return new PrioritizedChoice(
				mAnyCharacter(),
				mCharacterClass()
				)
				.Or(mCodePoint())
				.Or(mFATAL())
				.Or(mWARN())
				.Or(mLiteral())
				.Or(mDynamicBackReference())
				.Or(mRecursionCall())
				.Or(mCapturingGroup())
				.Or(mGroup());
		}


		private static AExpression mPrefix()
		{
			return new CapturingGroup("Prefix",
			                          new PrioritizedChoice(
			                          	new CapturingGroup("AndPredicate",
			                          	                   new Sequence(
			                          	                   	new Literal {MatchText = "&"}
			                          	                   	,
			                          	                   	new ZeroOrMore(mSpace())
			                          	                   	)
			                          	                   	.Sequence(mTerminal())
			                          		),
			                          	new CapturingGroup("NotPredicate",
			                          	                   new Sequence(
			                          	                   	new Literal {MatchText = "!"}
			                          	                   	,
			                          	                   	new ZeroOrMore(mSpace())
			                          	                   	)
			                          	                   	.Sequence(mTerminal())
			                          		)
			                          	)
				);
		}

		private static AExpression mSuffix()
		{
			return new CapturingGroup("Suffix",
			                          new PrioritizedChoice(
			                          	new CapturingGroup("ZeroOrMore",
			                          	                   new Sequence(
			                          	                   	mTerminal(),
			                          	                   	new ZeroOrMore(mSpace())
			                          	                   	)
			                          	                   	.Sequence(new Literal {MatchText = "*"})
			                          		),
			                          	new CapturingGroup("OneOrMore",
			                          	                   new Sequence(
			                          	                   	mTerminal(),
			                          	                   	new ZeroOrMore(mSpace())
			                          	                   	)
			                          	                   	.Sequence(new Literal {MatchText = "+"})
			                          		)
			                          	)
			                          	.Or(
			                          		new CapturingGroup("Optional",
			                          		                   new Sequence(
			                          		                   	mTerminal(),
			                          		                   	new ZeroOrMore(mSpace())
			                          		                   	)
			                          		                   	.Sequence(new Literal {MatchText = "?"})
			                          			)
			                          	)
			                          	.Or(
			                          		new CapturingGroup("LimitingRepetition",
			                          		                   new Sequence(
			                          		                   	mTerminal(),
			                          		                   	new ZeroOrMore(mSpace())
			                          		                   	)
			                          		                   	.Sequence(
			                          		                   		new Sequence(
			                          		                   			new Sequence(
			                          		                   				new Literal {MatchText = "{"},
			                          		                   				new ZeroOrMore(mSpace())
			                          		                   				),
			                          		                   			new PrioritizedChoice(
			                          		                   				// {min,max}
			                          		                   				new CapturingGroup("BETWEEN",
			                          		                   				                   new Sequence(
			                          		                   				                   	new CapturingGroup("Min",
			                          		                   				                   	                   new OneOrMore(
			                          		                   				                   	                   	new CharacterClass
			                          		                   				                   	                   		{
			                          		                   				                   	                   			ClassExpression =
			                          		                   				                   	                   				"[0-9]"
			                          		                   				                   	                   		})).Sequence(
			                          		                   				                   	                   			new ZeroOrMore(
			                          		                   				                   	                   				mSpace())),
			                          		                   				                   	new Literal {MatchText = ","}
			                          		                   				                   	)
			                          		                   				                   	.Sequence(
			                          		                   				                   		new Sequence(new ZeroOrMore(mSpace()),
			                          		                   				                   		             new CapturingGroup("Max",
			                          		                   				                   		                                new OneOrMore
			                          		                   				                   		                                	(new CharacterClass
			                          		                   				                   		                                	 	{
			                          		                   				                   		                                	 		ClassExpression
			                          		                   				                   		                                	 			=
			                          		                   				                   		                                	 			"[0-9]"
			                          		                   				                   		                                	 	})))
			                          		                   				                   	)
			                          		                   					)
			                          		                   				,
			                          		                   				//{,max}
			                          		                   				new CapturingGroup("ATMOST",
			                          		                   				                   new Sequence(
			                          		                   				                   	new Literal {MatchText = ","}
			                          		                   				                   	,
			                          		                   				                   	new Sequence(new ZeroOrMore(mSpace()),
			                          		                   				                   	             new CapturingGroup("Max",
			                          		                   				                   	                                new OneOrMore(
			                          		                   				                   	                                	new CharacterClass
			                          		                   				                   	                                		{
			                          		                   				                   	                                			ClassExpression
			                          		                   				                   	                                				= "[0-9]"
			                          		                   				                   	                                		})))
			                          		                   				                   	)
			                          		                   					)
			                          		                   				)
			                          		                   				.Or
			                          		                   				(
			                          		                   					//{min,}
			                          		                   				new CapturingGroup("ATLEAST",
			                          		                   				                   new Sequence(
			                          		                   				                   	new Sequence(new ZeroOrMore(mSpace()),
			                          		                   				                   	             new CapturingGroup("Min",
			                          		                   				                   	                                new OneOrMore(
			                          		                   				                   	                                	new CharacterClass
			                          		                   				                   	                                		{
			                          		                   				                   	                                			ClassExpression
			                          		                   				                   	                                				= "[0-9]"
			                          		                   				                   	                                		})))
			                          		                   				                   		.Sequence(new ZeroOrMore(mSpace()))
			                          		                   				                   	,
			                          		                   				                   	new Literal {MatchText = ","}
			                          		                   				                   	)
			                          		                   					)
			                          		                   				)
			                          		                   				.Or
			                          		                   				(
			                          		                   					new CapturingGroup("EXACT",
			                          		                   					                   new OneOrMore(new CharacterClass
			                          		                   					                                 	{ClassExpression = "[0-9]"}))
			                          		                   				)
			                          		                   			)
			                          		                   			.Sequence(
			                          		                   				new ZeroOrMore(mSpace())
			                          		                   			)
			                          		                   			.Sequence(
			                          		                   				new Literal {MatchText = "}"}
			                          		                   			)
			                          		                   	)
			                          			)
			                          	)
				);
		}


		private static AExpression mExpression()
		{
			return new PrioritizedChoice(
				// match prefixes first
				mPrefix()
				,
				// match suffixes next
				mSuffix()
				)
				.Or(
					mTerminal()
				);
		}


		private static AExpression mSequence()
		{
			return new CapturingGroup(
				"Sequence",
				new Sequence(
					mExpression(),
					new ZeroOrMore(mWhiteSpace())
					).Plus()
				) {DoReplaceBySingleChildNode = true};
			// DoReplaceBySingleChildNode = True; 
			// so Sequence captured group can become Terminal if only 1 child is created.  
			// The wild card makes this scenario possible.
		}

		private static AExpression mPrioritizedChoice()
		{
			return new CapturingGroup("PrioritizedChoice",
			                          new Sequence(
			                          	mSequence(), // Sequence consumes whitespace before / .. so no need to add it here again.
			                          	new Literal {MatchText = "/"}
			                          	)
			                          	.Sequence(new ZeroOrMore(mWhiteSpace()))
			                          	.Sequence(mSequence())
			                          	.Sequence(
			                          		new ZeroOrMore(
			                          			new Sequence(
			                          				new ZeroOrMore(mWhiteSpace()),
			                          				new Literal {MatchText = "/"}
			                          				)
			                          				.Sequence(new ZeroOrMore(mWhiteSpace()))
			                          				.Sequence(mSequence())
			                          				.Plus()
			                          			)
			                          	)
				);
		}

		private static AExpression mExpressionRoot()
		{
			return new RecursionCreate("ExpressionRoot", new PrioritizedChoice(mPrioritizedChoice(), mSequence()));
		}


		private static AExpression mStatement()
		{
			return new CapturingGroup(
				"Statement",
				new Sequence(
					new CapturingGroup("NodeDefinition",
					                   new PrioritizedChoice(
					                   	mLabel()
					                   	,
					                   	mTerminalReference()
					                   	)
						)
					,
					new Literal {MatchText = ":"}
					)
					.Sequence(new ZeroOrMore(mWhiteSpace()))
					.Sequence(mExpressionRoot())
					.Sequence(new ZeroOrMore(mWhiteSpace()))
					.Sequence(new Literal {MatchText = ";"})
				) {DoCreateCustomAstNode = true};
		}


		private static AExpression RootPegExpression()
		{
			return new CapturingGroup(
				"PEG",
				new Sequence(
					new Sequence(
						new ZeroOrMore(mWhiteSpace()),
						mStatement()
						)
						.Sequence(new ZeroOrMore(mWhiteSpace()))
						.Plus()
					,
					new NotPredicate(
						new AnyCharacter()
						)
					)
				) { DoCreateCustomAstNode = true };
		}


		public static AExpression Load(String rules)
		{
			var rootExpression = RootPegExpression();
			var iterator = new StringInputIterator(Encoding.UTF8.GetBytes(rules));
			var visitor = new NpegParserVisitor(iterator, new PeGrammarAstNodeFactory(iterator));

			rootExpression.Accept(visitor);
			if (visitor.IsMatch)
			{
				var interpret = (InterpreterAstNode)visitor.AST;
				return interpret.Expression;
			}

			throw new InvalidRuleException();
		}
	}
}