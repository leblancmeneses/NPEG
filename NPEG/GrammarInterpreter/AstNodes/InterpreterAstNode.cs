using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NPEG.Extensions;
using NPEG.NonTerminals;
using NPEG.Terminals;

namespace NPEG.GrammarInterpreter.AstNodes
{
	public class InterpreterAstNode : IAstNodeReplacement
	{
		private readonly IInputIterator _inputIterator;
		private readonly Dictionary<String, AExpression> completedStatements = new Dictionary<string, AExpression>();
		private readonly List<String> wrapWithRecursionRule = new List<string>();
		private AExpression expression;
		public Stack<AExpression> expressionStack = new Stack<AExpression>();


		private Boolean hasPassedNodeDefinition;

		public InterpreterAstNode(IInputIterator inputIterator)
		{
			_inputIterator = inputIterator;
		}

		public AExpression this[String terminalname]
		{
			get
			{
				if (completedStatements.ContainsKey(terminalname))
				{
					// a non terminal requesting a terminal
					return completedStatements[terminalname];
				}
				else
				{
					Boolean requestingRecursion = false;
					foreach (StatementAstNode node in Children)
					{
						if (node.Name == terminalname)
						{
							requestingRecursion = true;
						}
					}

					if (requestingRecursion)
					{
						// terminal wanting to recursively call a non terminal
						// that will be later defined in the document

						if (!wrapWithRecursionRule.Contains(terminalname))
							wrapWithRecursionRule.Add(terminalname);

						return new RecursionCall(terminalname);
					}
					else
					{
						// oops requesting terminal nowhere defined in the document.
						throw new InterpreterParseException("PEG Statement cannot be created.  Requesting '" +
						                                    terminalname +
						                                    "' terminal which is not defined in grammar rules.");
					}
				}
			}
		}

		public AExpression Expression
		{
			get
			{
				if (expression != null)
				{
					if (expression.GetType() == typeof (RecursionCreate))
					{
						if (((RecursionCreate) expression).TypeContains != typeof (CapturingGroup))
						{
							throw new InterpreterParseException(
								"Root terminal requires it be of type capturing group.  Wrap last terminal in (?<RootName>).");
						}
					}
					else if (expression.GetType() != typeof (CapturingGroup))
					{
						throw new InterpreterParseException(
							"Root terminal requires it be of type capturing group.  Wrap last terminal in (?<RootName>).");
					}
				}
				else
				{
					throw new InterpreterParseException("Unable to create expression.");
				}

				// assumes last terminal is root
				return expression;
			}
		}

		public override void VisitEnter(AstNode node)
		{
		}

		public override void VisitLeave(AstNode node)
		{
			AExpression left;
			AExpression right;

			if (hasPassedNodeDefinition)
			{
				switch (node.Token.Name)
				{
					case "Statement":
						hasPassedNodeDefinition = false;
						var statement = (StatementAstNode) node;

						if (statement.IsCaptured)
						{
							var captureStatement = new CapturingGroup(statement.Name, expressionStack.Pop()){RuleStart = node.Token.Start, RuleEnd = node.Token.End};
							if (
								statement.Children[0].Children[0].Children.Any(child => child.Token.Name == "OptionalFlags")
								&&
								statement.Children[0].Children[0].Children[1].Children.Any(child => child.Token.Name == "ReplaceBySingleChild")
								)
							{
								captureStatement.DoReplaceBySingleChildNode = true; // default is false
							}
							expression = captureStatement;
						}
						else
						{
							expression = expressionStack.Pop();
						}


						// Assumes Terminals are at the top of the file and 
						// final root non terminal expression is at the bottom.
						if (wrapWithRecursionRule.Contains(statement.Name))
						{
							expression = new RecursionCreate(statement.Name, expression){RuleStart = node.Token.Start, RuleEnd = node.Token.End};
						}

						completedStatements.Add(statement.Name, expression);

						break;
					case "Sequence":
						var reverse = new Stack<AExpression>();
						for (int i = 0; i < node.Children.Count; i++)
						{
							reverse.Push(expressionStack.Pop());
						}

						Decimal sequence_cnt = (decimal) node.Children.Count - 1;
						for (; sequence_cnt > 0; sequence_cnt--)
						{
							left = reverse.Pop();
							right = reverse.Pop();
							reverse.Push(
								new Sequence(left, right) { RuleStart = node.Token.Start, RuleEnd = node.Token.End }
								);
						}

						expressionStack.Push(reverse.Pop());

						break;
					case "PrioritizedChoice":
						Int32 cnt = node.Children.Count - 1;
						for (Int32 i = 0; i < cnt; i++)
						{
							right = expressionStack.Pop();
							left = expressionStack.Pop();

							expressionStack.Push(
								new PrioritizedChoice(left, right) { RuleStart = node.Token.Start, RuleEnd = node.Token.End }
								);
						}
						break;


					case "Prefix":
						switch (node.Token.ValueAsString(_inputIterator)[0].ToString())
						{
							case "!":
								expressionStack.Push(new NotPredicate(expressionStack.Pop()) { RuleStart = node.Token.Start, RuleEnd = node.Token.End });
								break;
							case "&":
								expressionStack.Push(new AndPredicate(expressionStack.Pop()) { RuleStart = node.Token.Start, RuleEnd = node.Token.End });
								break;
							default:
								throw new Exception("Unsupported PEG Prefix.");
						}
						break;

					case "Suffix":
						switch (node.Children[0].Token.Name)
						{
							case "ZeroOrMore":
								expressionStack.Push(new ZeroOrMore(expressionStack.Pop()) { RuleStart = node.Token.Start, RuleEnd = node.Token.End });
								break;
							case "OneOrMore":
								expressionStack.Push(new OneOrMore(expressionStack.Pop()) { RuleStart = node.Token.Start, RuleEnd = node.Token.End });
								break;
							case "Optional":
								expressionStack.Push(new Optional(expressionStack.Pop()) { RuleStart = node.Token.Start, RuleEnd = node.Token.End });
								break;
							case "LimitingRepetition":
								switch (node.Children[0].Children[1].Token.Name)
								{
									case "BETWEEN":
										expressionStack.Push(new LimitingRepetition(expressionStack.Pop())
										                     	{
										                     		Min =
										                     			Int32.Parse(
										                     				node.Children[0].Children[1].Children[0].
																				Token.ValueAsString(_inputIterator)),
										                     		Max =
										                     			Int32.Parse(
										                     				node.Children[0].Children[1].Children[1].
																				Token.ValueAsString(_inputIterator)),
																	RuleStart = node.Token.Start,
																	RuleEnd = node.Token.End
										                     	});
										break;
									case "ATMOST":
										expressionStack.Push(new LimitingRepetition(expressionStack.Pop())
										                     	{
										                     		Min = null,
										                     		Max = Int32.Parse(node.Children[0].Children[1].Children[0].Token
																				.ValueAsString(_inputIterator)),
																	RuleStart = node.Token.Start,
																	RuleEnd = node.Token.End
										                     	});
										break;
									case "ATLEAST":
										expressionStack.Push(new LimitingRepetition(expressionStack.Pop())
										                     	{
										                     		Min =
										                     			Int32.Parse(node.Children[0].Children[1].Children[0].
																				Token.ValueAsString(_inputIterator)),
																	Max = null,
																	RuleStart = node.Token.Start,
																	RuleEnd = node.Token.End
										                     	});
										break;
									case "EXACT":
										Int32 exactcount = Int32.Parse(node.Children[0].Children[1].Token.ValueAsString(_inputIterator));
										expressionStack.Push(new LimitingRepetition(expressionStack.Pop()) { Min = exactcount, Max = exactcount, RuleStart = node.Token.Start, RuleEnd = node.Token.End });
										break;
									case "VariableLength":
										var variableLengthExpression = node.Children[0].Children[1].Token.ValueAsString(_inputIterator);
										expressionStack.Push(new LimitingRepetition(expressionStack.Pop()) { VariableLengthExpression = variableLengthExpression, RuleStart = node.Token.Start, RuleEnd = node.Token.End });
										break;
								}
								break;
							default:
								throw new Exception("Unsupported PEG Suffix.");
						}
						break;


					case "CapturingGroup":
						var capture = new CapturingGroup(node.Children[0].Token.ValueAsString(_inputIterator), expressionStack.Pop()) { RuleStart = node.Token.Start, RuleEnd = node.Token.End };

						if (node.Children.Any(child => child.Token.Name == "OptionalFlags"))
						{
							if (node.Children[1].Children.Any(child => child.Token.Name == "ReplaceBySingleChild"))
							{
								capture.DoReplaceBySingleChildNode = true; // default is false
							}
							if (node.Children[1].Children.Any(child => child.Token.Name == "ReplacementNode"))
							{
								capture.DoCreateCustomAstNode = true; // default is false
							}
						}
						expressionStack.Push(capture);
						break;
					case "Group":
						break;
					case "AnyCharacter":
						expressionStack.Push(new AnyCharacter() { RuleStart = node.Token.Start, RuleEnd = node.Token.End });
						break;
					case "Literal":
						Boolean isCaseSensitive = true;
						if (node.Children.Count == 2)
							isCaseSensitive = false;

						expressionStack.Push(new Literal
						                     	{
						                     		IsCaseSensitive = isCaseSensitive,
						                     		MatchText = Regex.Replace(
														Regex.Replace(node.Children[0].Token.ValueAsString(_inputIterator), @"\\(?<quote>""|')", @"${quote}")
						                     			, @"\\\\", @"\"),
													RuleStart = node.Token.Start,
													RuleEnd = node.Token.End
						                     	});
						break;
					case "CharacterClass":
						expressionStack.Push(new CharacterClass { ClassExpression = node.Token.ValueAsString(_inputIterator), RuleStart = node.Token.Start, RuleEnd = node.Token.End });
						break;
					case "RecursionCall":
						expressionStack.Push((this)[node.Children[0].Token.ValueAsString(_inputIterator)]);
						break;
					case "CodePoint":
						expressionStack.Push(new CodePoint { Match = "#" + node.Children[0].Token.ValueAsString(_inputIterator), RuleStart = node.Token.Start, RuleEnd = node.Token.End });
						break;
					case "Fatal":
						expressionStack.Push(new Fatal { Message = node.Children[0].Token.ValueAsString(_inputIterator), RuleStart = node.Token.Start, RuleEnd = node.Token.End });
						break;
					case "Warn":
						expressionStack.Push(new Warn { Message = node.Children[0].Token.ValueAsString(_inputIterator), RuleStart = node.Token.Start, RuleEnd = node.Token.End });
						break;
					case "DynamicBackReferencing":
						if (node.Children.Count == 1)
						{
							// no options specified only tag name.
							expressionStack.Push(new DynamicBackReference { BackReferenceName = node.Children[0].Token.ValueAsString(_inputIterator), RuleStart = node.Token.Start, RuleEnd = node.Token.End });
						}
						else
						{
							throw new NotImplementedException(
								"Add IsCaseSensitive using children[1].Token.Name == IsCasesensitive");
						}
						break;
				}
			}

			if (node.Token.Name == "NodeDefinition")
			{
				hasPassedNodeDefinition = true;
			}
		}
	}
}