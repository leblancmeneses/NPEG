using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NPEG.Algorithms;
using NPEG.ApplicationExceptions;
using NPEG.GrammarInterpreter;
using NPEG.NonTerminals;
using NPEG.Terminals;

namespace NPEG
{
	public class NpegParserVisitor : IParseTreeVisitor
	{
		private readonly Stack<IsMatchPredicate> _matchStack = new Stack<IsMatchPredicate>();
		// helps to incrementally build a deeply nested parse tree.

		public delegate Boolean IsMatchPredicate(IInputIterator iterator);

		// function pointer all functions inserted into matchStack must match this interface.
		private readonly Dictionary<String, IsMatchPredicate> _recursiveMethod = new Dictionary<string, IsMatchPredicate>();

		// Inserted by RecursionCreate;  It's role is to match a piece of a parse tree to a label (dictionary key)
		// Consumed by RecursionCall; Send iterator down a named parse tree path.
		// This arrangement allows grammar to support recursiveness.
		// Value: [0-9]+ / '(' Expr ')';
		// Expr: Value;


		private readonly Stack<Boolean> _disableCapturingGroup = new Stack<bool>();
		// This stack helps manage when CapturingGroup is added to the parse tree.
		// All capturinggroups are ignored when located inside AndPredicate and NotPredicate as predicates do not consume characters.
		// When not in an AndPredicate or NotPredicate capturing groups are added to the parse tree.


		protected IInputIterator iterator;

		#region Xml Parsing

		private readonly Dictionary<String, Stack<Byte[]>> _xmlBackReferenceLookup = new Dictionary<string, Stack<Byte[]>>();
		// Currently specific to xml in implementation.. (if other applications are found.. we could extend this)
		// -- not sure if dynamic reference should be a stack, maybe we shouldn't be popping either as it might need to be referenced several times?
		// The sequence for this to work: 
		// 1) CapturingGroup must be used which adds name as key and adds captured string.
		// 2) DynamicReference can later be used to compare previously consumed characters with existing pointer located strings.
		// '<' (?<tag> . . ) '>'  '</' \k<tag[\i]> '>'
		// could match: <h1> </H1>
		// capture first; later consumed

		private readonly Stack<Boolean> _xmlDisableBackReferencePop = new Stack<bool>();
		// Helps DynamicBackReference since this method pops elements off (element can only be referenced once)
		// When inside an AndPredicate or NotPredicate we don't want the stack to be minimized since predicates don't consume characters. 
		// Therefore disableBackReferencePop helps identify when elements should be fetched with peek or with pop.

		#endregion
		
		private readonly Stack<Stack<AstNode>> _sandbox = new Stack<Stack<AstNode>>();
		// Stack used to build AST.
		// var s = new Stack<Stack<AstNode>>()
		// s.Push(new Stack<AstNode>());  
		// Is the sandbox context which CapturingGroup works in.
		// =============================
		// Ideas behind this structure:
		// =============================
		// Suffix: (?<ZeroOrMore> '*') / (?<OneOrMore> '+') / (?<Optional> '?');
		// Terminal: (?<AnyCharacter> '.');
		// (?<Expression>): Terminal Suffix+ / Terminal+;
		// If CapturingGroup was the only nonterminal managing the ast stack, 
		// given "." as input would cause the final ast to be: 1 Expression node with nested 2 AnyCharacter nodes. 
		// When in fact we would expect: 1 Expression node with nested 1 AnyCharacter node
		// Hence, PriorityChoice needs to have the capability to provide a stack which CapturingGroup works in so when priority choice identifies a branch 
		// was not completed the context can be removed and bogus captured items are destroyed
		// So although CapturingGroup does most of the work to populate the ast.  
		// PriorityChoice provides the sandbox in which it works.


		private readonly IAstNodeFactory _astNodeFactory;

		public NpegParserVisitor(IInputIterator iterator)
		{
			this.iterator = iterator;
			Warnings = new List<Warn>();
			_sandbox.Push(new Stack<AstNode>()); // initial base sandbox
			IsOptimized = false;
		}

		public NpegParserVisitor(IInputIterator iterator, IAstNodeFactory astNodeFactory)
		{
			this.iterator = iterator;
			Warnings = new List<Warn>();
			_sandbox.Push(new Stack<AstNode>()); // initial base sandbox
			IsOptimized = false;

			_astNodeFactory = astNodeFactory;
		}


		public Boolean IsOptimized
		// if rules do not require saving dynamic back reference
		// do not allocate space for them.
		{ get; set; }


		public AstNode AST
		{
			get
			{
				if (
					_sandbox.Count == 1
					// All sandboxes have been succesfully reduced into 1
					&&
					_sandbox.Peek().Count == 1
					// The ast in the last sandbox is complete because it contains only 1 astnode.
					)
				{
					return _sandbox.Peek().Peek();
				}
				else
				{
					return null;
				}
			}
		}

		private Boolean _hasBeenMatched;
		private Boolean _matchedValue;

		public Boolean IsMatch
		{
			get
			{
				if (!_hasBeenMatched)
				{
					_hasBeenMatched = true;
					_matchedValue = _matchStack.Peek()(iterator);
				}

				return _matchedValue;
			}
		}

		public List<Warn> Warnings { get; set; }

		#region nonterminals - must save iterator position.

		public override void VisitEnter(AndPredicate expression)
		{
		}

		public override void VisitExecute(AndPredicate expression)
		{
		}

		public override void VisitLeave(AndPredicate expression)
		{
			IsMatchPredicate exp = _matchStack.Pop();
			_matchStack.Push(
				delegate(IInputIterator iterator)
				{
					_disableCapturingGroup.Push(true);

					_xmlDisableBackReferencePop.Push(true);
					Boolean result = true;
					Int32 savePosition = iterator.Index;
					if (exp(iterator))
					{
						iterator.Index = savePosition;
						result &= true;
					}
					else
					{
						iterator.Index = savePosition;
						result &= false;
					}

					_xmlDisableBackReferencePop.Pop();

					_disableCapturingGroup.Pop();

					return result;
				}
			);
		}

		public override void VisitEnter(NotPredicate expression)
		{
		}

		public override void VisitExecute(NotPredicate expression)
		{
		}

		public override void VisitLeave(NotPredicate expression)
		{
			IsMatchPredicate local = _matchStack.Pop();
			_matchStack.Push(
				delegate(IInputIterator iterator)
				{
					_disableCapturingGroup.Push(true);

					_xmlDisableBackReferencePop.Push(true);

					Boolean result = true;
					Int32 savePosition = iterator.Index;
					if (!local(iterator))
					{
						iterator.Index = savePosition;
						result &= true;
					}
					else
					{
						iterator.Index = savePosition;
						result &= false;
					}

					_xmlDisableBackReferencePop.Pop();

					_disableCapturingGroup.Pop();

					return result;
				}
			);
		}


#warning create another thread and process these simultaneously.
		public override void VisitEnter(PrioritizedChoice expression)
		{
		}

		public override void VisitExecute(PrioritizedChoice expression)
		{
		}

		public override void VisitLeave(PrioritizedChoice expression)
		{
			IsMatchPredicate localRight = _matchStack.Pop();
			IsMatchPredicate localLeft = _matchStack.Pop();
			_matchStack.Push(
				delegate(IInputIterator iterator)
				{
					Int32 savePosition = iterator.Index;

					_sandbox.Push(new Stack<AstNode>());
					_sandbox.Peek().Push(new AstNode());
					// create new sandbox which capturing group works in.
					// have a dummy node to act as parent container (CapturedGroup logic)

					if (localLeft(iterator))
					{
						Stack<AstNode> s = _sandbox.Pop();
						if (s.Count >= 1)
						{
							AstNode child = s.Pop();
							if (_sandbox.Peek().Count == 0)
							{
								_sandbox.Peek().Push(child);
							}
							else
							{
								_sandbox.Peek().Peek().Children.AddRange(child.Children);
								// our dummy node for this sandbox is child.
								// move important data over to the previous sandbox.
							}
						}

						return true;
					}

					_sandbox.Pop();
					// destory sandbox since anything captured is no longer valid.


					iterator.Index = savePosition;


					_sandbox.Push(new Stack<AstNode>());
					_sandbox.Peek().Push(new AstNode());
					// create new sandbox which capturing group works in.
					// have a dummy node to act as parent container (CapturedGroup logic)

					if (localRight(iterator))
					{
						Stack<AstNode> s = _sandbox.Pop();
						if (s.Count >= 1)
						{
							AstNode child = s.Pop();
							if (_sandbox.Peek().Count == 0)
							{
								_sandbox.Peek().Push(child);
							}
							else
							{
								_sandbox.Peek().Peek().Children.AddRange(child.Children);
								// our dummy node for this sandbox is child.
								// move important data over to the previous sandbox.
							}
						}

						return true;
					}

					_sandbox.Pop();
					// destory sandbox since anything captured is no longer valid.

					return false;
				}
				);
		}

		public override void VisitEnter(ZeroOrMore expression)
		{
		}

		public override void VisitExecute(ZeroOrMore expression)
		{
		}

		public override void VisitLeave(ZeroOrMore expression)
		{
			IsMatchPredicate local = _matchStack.Pop();
			_matchStack.Push(
				delegate(IInputIterator iterator)
				{
					Int32 savePosition = iterator.Index;
					while (local(iterator))
					{
						if (savePosition == iterator.Index)
						{
							//Exception ex = new InfiniteLoopDetectedException();
							//ex.Data.Add("Iterator.Index", iterator.Index);
							//var rulewriter = new WriteRuleVisitor();
							//expression.Accept(rulewriter);
							//ex.Data.Add("Expression", rulewriter.GrammarOutput);
							//throw ex;
							break;
						}
						savePosition = iterator.Index;
					}

					iterator.Index = savePosition;

					return true;
				}
				);
		}

		public override void VisitEnter(OneOrMore expression)
		{
		}

		public override void VisitExecute(OneOrMore expression)
		{
		}

		public override void VisitLeave(OneOrMore expression)
		{
			IsMatchPredicate local = _matchStack.Pop();
			_matchStack.Push(
				delegate(IInputIterator iterator)
				{
					Int32 cnt = 0;
					Int32 savePosition = iterator.Index;
					while (local(iterator))
					{
						cnt++;
						if (savePosition == iterator.Index)
						{
							//InfiniteLoopDetectedException
							break;
						}
						savePosition = iterator.Index;
					}

					iterator.Index = savePosition;

					return (cnt > 0);
				}
				);
		}

		public override void VisitEnter(Optional expression)
		{
		}

		public override void VisitExecute(Optional expression)
		{
		}

		public override void VisitLeave(Optional expression)
		{
			IsMatchPredicate local = _matchStack.Pop();
			_matchStack.Push(
				delegate(IInputIterator iterator)
				{
					Int32 savePosition = iterator.Index;
					if (local(iterator))
					{
						savePosition = iterator.Index;
					}
					else
					{
						iterator.Index = savePosition;
					}

					return true;
				}
				);
		}


		public override void VisitEnter(LimitingRepetition expression)
		{
		}

		public override void VisitExecute(LimitingRepetition expression)
		{
		}

		public override void VisitLeave(LimitingRepetition expression)
		{
			Int32? max = expression.Max;
			Int32? min = expression.Min;
			IsMatchPredicate local = _matchStack.Pop();

			_matchStack.Push(
				delegate(IInputIterator iterator)
				{
					Int32 cnt = 0;
					Int32 savePosition = iterator.Index;
					Boolean result = false;
					if (expression.VariableLengthExpression == null)
					{
						if (min != null)
						{
							if (max == null)
							{
								// has a minimum but no max cap
								savePosition = iterator.Index;
								while (local(iterator))
								{
									cnt++;
									if (savePosition == iterator.Index)
									{
										//InfiniteLoopDetectedException
										break;
									}
									savePosition = iterator.Index;
								}

								iterator.Index = savePosition;
								result = (cnt >= min);
							}
							else
							{
								// has a minimum and a max specified

								if (max < min)
								{
									throw new ArgumentException(
										"A Max property must be larger than Min when using LimitingRepetition.");
								}

								savePosition = iterator.Index;
								while (local(iterator))
								{
									cnt++;
									savePosition = iterator.Index;

									if (cnt >= max)
									{
										break;
									}
								}

								iterator.Index = savePosition;
								result = (cnt <= max && cnt >= min);
							}
						}
						else // min == null
						{
							if (max == null)
							{
								throw new ArgumentException(
									"A Min and/or Max must be specified when using LimitingRepetition.");
							}

							// zero or up to a max matches of e.
							savePosition = iterator.Index;
							while (local(iterator))
							{
								cnt++;
								savePosition = iterator.Index;

								if (cnt >= max)
								{
									break;
								}
							}

							iterator.Index = savePosition;
							result = (cnt <= max);
						}
					}
					else
					{
						var variableValues = _sandbox.Peek().ToArray().SelectMany(x=>x.Children).ToList();

						var varLengthBytes = Encoding.UTF8.GetBytes(expression.VariableLengthExpression);
						var varLengthIterator = new ByteInputIterator(varLengthBytes);

						var varLengthParser = new LimitingRepetitionVariableLengthExpressionParser();
						var varLengthParseTree = varLengthParser.Parse(varLengthIterator);

						var varLengthSolver = new LimitingRepetitionVariableLengthExpressionSolver(varLengthIterator, iterator, variableValues);
						varLengthParseTree.Accept(varLengthSolver);


						var value = (int)varLengthSolver.Result;
						savePosition = iterator.Index;
						while (value > cnt && local(iterator))
						{
							cnt++;
							savePosition = iterator.Index;

							if (cnt == value)
							{
								break;
							}
						}

						iterator.Index = savePosition;
						result = (cnt == value);

					}
					return result;
				}
				);
		}


		public override void VisitEnter(Sequence expression)
		{
		}

		public override void VisitExecute(Sequence expression)
		{
		}

		public override void VisitLeave(Sequence expression)
		{
			IsMatchPredicate localRight = _matchStack.Pop();
			IsMatchPredicate localLeft = _matchStack.Pop();
			_matchStack.Push(
				delegate(IInputIterator iterator)
				{
					Boolean result = true;
					Int32 savePosition = iterator.Index;
					if (localLeft(iterator) && localRight(iterator))
					{
						result &= true;
					}
					else
					{
						iterator.Index = savePosition;
						result &= false;
					}

					return result;
				}
				);
		}


		public override void VisitEnter(CapturingGroup expression)
		{
		}

		public override void VisitExecute(CapturingGroup expression)
		{
		}

		public override void VisitLeave(CapturingGroup expression)
		{
			String name = expression.Name;
			Boolean reduceBySingleChildNode = expression.DoReplaceBySingleChildNode;
			IsMatchPredicate local = _matchStack.Pop();

			_matchStack.Push(
				delegate(IInputIterator iterator)
				{
					if (expression.DoCreateCustomAstNode && _astNodeFactory == null)
					{
						throw new ArgumentNullException("Second constructor overload is required during instantiation.  astNodeFactory requires to be set with this parser implementation.");
					}


					Int32 savePosition = iterator.Index;
					_sandbox.Peek().Push(new AstNode());

					if (local(iterator))
					{
						// predicates being processed should not append ast
						if (_disableCapturingGroup.Count > 0)
						{
							_sandbox.Peek().Pop();
							iterator.Index = savePosition;
							return true;
						}

						if (savePosition >= iterator.Index)
						{
							// Warn terminal does not consume and ast should not be created for it, yet it should return that it was successful match.
							_sandbox.Peek().Pop();
							iterator.Index = savePosition;
							return true;
						}


						Byte[] matchedBytes = iterator.Text(savePosition, iterator.Index - 1);
						if (_xmlBackReferenceLookup.ContainsKey(name))
						{
							_xmlBackReferenceLookup[name].Push(matchedBytes);
						}
						else
						{
							_xmlBackReferenceLookup.Add(name, new Stack<Byte[]>());
							_xmlBackReferenceLookup[name].Push(matchedBytes);
						}
							

						AstNode node = _sandbox.Peek().Pop();
						node.Token = new TokenMatch(name, savePosition, iterator.Index - 1);


						if (expression.DoCreateCustomAstNode)
						{
							// create a custom astnode
							IAstNodeReplacement nodevisitor = _astNodeFactory.Create(node);
							nodevisitor.Token = node.Token;
							nodevisitor.Parent = node.Parent;
							nodevisitor.Children = node.Children;
							foreach (AstNode updateparent in nodevisitor.Children)
							{
								updateparent.Parent = nodevisitor;
							}

							// since the whole tree has not finished completing this.Parent will be null on this run.
							// logic inside astnodereplacement is to create properties, business names, that internally check Children collection to mine data.
							// you still will need a top level visitor to process tree after it completely available.
							node.Accept(nodevisitor);
							node = nodevisitor;
						}


						if (reduceBySingleChildNode)
						{
							if (node.Children.Count == 1)
							{
								node = node.Children[0];
							}
						}


						if (_sandbox.Peek().Count > 0)
						{
							node.Parent = _sandbox.Peek().Peek();
							_sandbox.Peek().Peek().Children.Add(node);
						}
						else
						{
							_sandbox.Peek().Push(node);
							// don't loose the root node
							// each successful sandbox will have 1 item left in the stack
						}

						return true;
					}
					_sandbox.Peek().Pop();
					iterator.Index = savePosition;
					return false;
			});
		}

		public override void VisitEnter(RecursionCreate expression)
		{
		}

		public override void VisitExecute(RecursionCreate expression)
		{
		}

		public override void VisitLeave(RecursionCreate expression)
		{
			IsMatchPredicate local = _matchStack.Pop();
			if (!_recursiveMethod.ContainsKey(expression.FunctionName))
				_recursiveMethod.Add(
					expression.FunctionName
					,
					delegate(IInputIterator iterator) { return local(iterator); }
				);

			_matchStack.Push(_recursiveMethod[expression.FunctionName]);
		}

		#endregion

		#region terminals - consume characters

		public override void Visit(Literal expression)
		{
			Boolean iscasesensitive = expression.IsCaseSensitive;
			var bytes = Encoding.UTF8.GetBytes(expression.MatchText);
			_matchStack.Push(
				delegate(IInputIterator iterator)
				{
					foreach (var c in bytes)
					{
						if (iscasesensitive)
						{
							if (iterator.Current() == -1)
								return false;

							if ((Byte)iterator.Current() != c)
							{
								iterator.Next();
								return false;
							}
						}
						else
						{
							if (iterator.Current() == -1)
								return false;

							if (!String.Equals(Encoding.UTF8.GetString(new[] { (Byte)iterator.Current() }, 0, 1), Encoding.UTF8.GetString(new[] { c }, 0, 1), StringComparison.CurrentCultureIgnoreCase))
							{
								iterator.Next();
								return false;
							}
						}

						iterator.Next();
					}

					return true;
				});
		}

		public override void Visit(CodePoint expression)
		{
			String matchtext = expression.Match;

			_matchStack.Push(
				delegate(IInputIterator iterator)
				{
					if (matchtext.StartsWith("#x"))
					{
						//hexadecimal evaluation
						String hexinput = matchtext.Substring(2);
						if (hexinput == String.Empty)
							throw new ArgumentException("Hex value specified is empty.");
						if (hexinput.Length % 2 != 0)
							hexinput = "0" + hexinput; // on incomplete byte boundaries shift right

						int total_compare_bytes = hexinput.Length / 2;

						for (int i = 0; i < total_compare_bytes; i++)
						{
							if (iterator.Current() == -1)
								return false;

							var compareWith = (Byte)iterator.Current();

							for (Int32 j = 0; j < 2; j++)
							{
								// low nibble comparison
								Int32 nibbleOffset = 0;
								if (j == 0)
								{
									// high nibble comparison
									nibbleOffset = 4;
								}

								// j == 0 == high nibble
								// j == 1 == low nibble
								var tmp = (Byte)hexinput[i * 2 + j];
								if (tmp >= (Byte)'0' && tmp <= (Byte)'9')
								{
									tmp = (Byte)((tmp - (Byte)'0') << nibbleOffset);
								}
								else if (tmp >= (Byte)'a' && tmp <= (Byte)'f')
								{
									tmp = (Byte)((tmp - (Byte)'A' + 10) << nibbleOffset);
								}
								else if (tmp >= (Byte)'A' && tmp <= (Byte)'F')
								{
									tmp = (Byte)((tmp - (Byte)'A' + 10) << nibbleOffset);
								}
								else if (tmp == (Byte)'x' || tmp == (Byte)'X')
								{
									// don't care ignore nibble comparison
								}
								else
								{
									throw new ArgumentException("Hex value specified contains invalid characters.");
								}

								if (!(tmp == (Byte)'x' || tmp == (Byte)'X'))
								{
									if (
										((compareWith & (1 << (nibbleOffset + 3))) >> (nibbleOffset + 3)) !=
										((tmp & (1 << (nibbleOffset + 3))) >> (nibbleOffset + 3)) ||
										((compareWith & (1 << (nibbleOffset + 2))) >> (nibbleOffset + 2)) !=
										((tmp & (1 << (nibbleOffset + 2))) >> (nibbleOffset + 2)) ||
										((compareWith & (1 << (nibbleOffset + 1))) >> (nibbleOffset + 1)) !=
										((tmp & (1 << (nibbleOffset + 1))) >> (nibbleOffset + 1)) ||
										((compareWith & (1 << nibbleOffset)) >> nibbleOffset) !=
										((tmp & (1 << nibbleOffset)) >> nibbleOffset)
										)
									{
										return false;
									}
								}
							}

							iterator.Next();
						}
					}
					else if (matchtext.StartsWith("#b"))
					{
						// binary evaluation
						String binaryinput = matchtext.Substring(2);
						if (binaryinput == String.Empty)
							throw new ArgumentException("Binary value specified is empty.");
						if (binaryinput.Length % 8 != 0)
							binaryinput = "".PadLeft(8 - (binaryinput.Length % 8), '0') + binaryinput;
						// on incomplete byte boundaries shift right

						int total_compare_bytes = binaryinput.Length / 8;

						for (int i = 0; i < total_compare_bytes; i++)
						{
							if (iterator.Current() == -1)
								return false;

							var compareWith = (Byte)iterator.Current();
							for (Int32 j = 0; j < 8; j++)
							{
								var tmp = (Byte)binaryinput[i * 8 + j];
								if (tmp == (Byte)'0')
								{
									tmp = 0x00;
								}
								else if (tmp == (Byte)'1')
								{
									tmp = 0x01;
								}
								else if (tmp == (Byte)'x' || tmp == (Byte)'X')
								{
									// don't care ignore nibble comparison
								}
								else
								{
									throw new ArgumentException(
										"Binary value specified contains invalid characters.");
								}

								if (!(tmp == (Byte)'x' || tmp == (Byte)'X'))
								{
									Int32 bit2compare = 7 - j;
									if (
										((compareWith & (1 << bit2compare)) >> bit2compare) != tmp
										)
									{
										return false;
									}
								}
							}

							iterator.Next();
						}
					}
					else
					{
						// decimal codepoint evaluation
						String decimalinput = matchtext.Substring(1);
						if (decimalinput == String.Empty)
							throw new ArgumentException("Decimal codepoint value specified is empty.");
						if (!Regex.IsMatch(decimalinput, @"^[0-9]+$"))
							throw new ArgumentException(
								"Decimal codepoint value specified contains invalid characters.");
						if (decimalinput.Length > 10) // 4,294,967,295
							throw new ArgumentException(
								"Decimal codepoint value exceeds 4 byte maximum length.  Largest decimal value possible 2^32");

						UInt32 codepoint = UInt32.Parse(decimalinput);
						var c4 = (Byte)(codepoint >> 24);
						var c3 = (Byte)(codepoint >> 16);
						var c2 = (Byte)(codepoint >> 8);
						var c1 = (Byte)(codepoint);
						if (c4 != 0x00)
						{
							// consumes 4 bytes
							if (iterator.Current() == -1 || iterator.Current() != c1)
							{
								return false;
							}
							iterator.Next();
							if (iterator.Current() == -1 || iterator.Current() != c2)
							{
								return false;
							}
							iterator.Next();
							if (iterator.Current() == -1 || iterator.Current() != c3)
							{
								return false;
							}
							iterator.Next();
							if (iterator.Current() == -1 || iterator.Current() != c4)
							{
								return false;
							}
							iterator.Next();
						}
						else if (c3 != 0x00)
						{
							// consumes 3 bytes
							if (iterator.Current() == -1 || iterator.Current() != c1)
							{
								return false;
							}
							iterator.Next();
							if (iterator.Current() == -1 || iterator.Current() != c2)
							{
								return false;
							}
							iterator.Next();
							if (iterator.Current() == -1 || iterator.Current() != c3)
							{
								return false;
							}
							iterator.Next();
						}
						else if (c2 != 0x00)
						{
							// consumes 2 bytes
							if (iterator.Current() == -1 || iterator.Current() != c1)
							{
								return false;
							}
							iterator.Next();
							if (iterator.Current() == -1 || iterator.Current() != c2)
							{
								return false;
							}
							iterator.Next();
						}
						else
						{
							// user must want to match null \0
							// (c1 != 0x00)
							// consumes 1 byte
							if (iterator.Current() == -1 || iterator.Current() != c1)
							{
								return false;
							}
							iterator.Next();
						}
					}

					return true;
				}
				);
		}


		public override void Visit(AnyCharacter expression)
		{
			_matchStack.Push(
				delegate(IInputIterator iterator)
				{
					// returns true only if this method can consume a character.
					if (iterator.Index < iterator.Length)
					{
						/*
						 * Consume one character since we're neither at the end of a real string, 
						 * nor operating on the empty string.
						 */
						iterator.Index += 1;
						return true;
					}
					else
					{
						return false;
					}
				}
				);
		}

		public override void Visit(CharacterClass expression)
		{
			String classExpression = expression.ClassExpression;
			_matchStack.Push(
				delegate(IInputIterator iterator)
				{
					if (iterator.Current() == -1)
						return false;

					var builder = new StringBuilder();
					builder.Append((Char)iterator.Current());
					Boolean result = Regex.IsMatch(builder.ToString(), classExpression);
					if (result)
					{
						iterator.Next();
					}
					return result;
				}
				);
		}

		public override void Visit(RecursionCall expression)
		{
			String delegatePointer = expression.FunctionName;
			_matchStack.Push(
				delegate(IInputIterator iterator) { return _recursiveMethod[delegatePointer](iterator); }
				);
		}


		public override void Visit(DynamicBackReference expression)
		{
			String backreferencename = expression.BackReferenceName;
			Boolean isCaseSensitive = expression.IsCaseSensitive;

			_matchStack.Push(
				delegate(IInputIterator iterator)
				{
					var matchBytes = new Byte[1];
					if (_xmlDisableBackReferencePop.Count <= 0)
						matchBytes = _xmlBackReferenceLookup[backreferencename].Pop();
					else
						matchBytes = _xmlBackReferenceLookup[backreferencename].Peek();


					foreach (Byte c in matchBytes)
					{
						if (isCaseSensitive)
						{
							if (iterator.Current() != c)
							{
								iterator.Next();
								return false;
							}
						}
						else
						{
							var builder = new StringBuilder();
							builder.Append((Char)iterator.Current());
							if (builder.ToString().ToUpper() != c.ToString().ToUpper())
							{
								iterator.Next();
								return false;
							}
						}
						iterator.Next();
					}

					return true;
				}
				);
		}


		public override void Visit(Warn expression)
		{
			_matchStack.Push(
				delegate(IInputIterator iterator)
				{
					expression.Position = iterator.Index;
					Warnings.Add(expression);
					return true;
				}
				);
		}

		public override void Visit(Fatal expression)
		{
			_matchStack.Push(
				delegate(IInputIterator iterator)
				{
					Exception fatal = new ParsingFatalTerminalException(expression.Message);
					fatal.Data.Add("Iterator.Position", iterator.Index);
					throw fatal;
				}
				);
		}

		#endregion
	}
}