using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace RobustHaven.Text.Npeg
{
	public delegate Boolean IsMatchPredicate();

	public abstract class NpegParser
	{
		public List<Warn> Warnings = new List<Warn>();

		protected InputIterator iterator;

		private readonly IAstNodeFactory pointerAstNodeFactory;
		private readonly Stack<Stack<AstNode>> sandbox = new Stack<Stack<AstNode>>();
		private readonly Dictionary<String, Stack<Byte[]>> backReferenceLookup = new Dictionary<string, Stack<Byte[]>>();
		private readonly Stack<Boolean> disableBackReferencePop = new Stack<bool>();


		public NpegParser(InputIterator iterator)
		{
			this.iterator = iterator;
			sandbox.Push(new Stack<AstNode>());
		}

		public NpegParser(InputIterator iterator, IAstNodeFactory createCustomAstNode)
		{
			this.iterator = iterator;
			pointerAstNodeFactory = createCustomAstNode;
			sandbox.Push(new Stack<AstNode>());
		}

		public abstract Boolean IsMatch();

		public AstNode getAST()
		{
			if (
				sandbox.Count == 1
				// All sandboxes have been succesfully reduced into 1
				&&
				sandbox.Peek().Count == 1
				// The ast in the last sandbox is complete because it contains only 1 astnode.
				)
			{
				return sandbox.Peek().Peek();
			}
			else
			{
				return null;
			}
		}


		protected Boolean AndPredicate(IsMatchPredicate predicate)
		{
			Boolean result = true;
			disableBackReferencePop.Push(true);
			Int32 savePosition = iterator.Index;
			if (predicate())
			{
				iterator.Index = savePosition;
				result &= true;
			}
			else
			{
				iterator.Index = savePosition;
				result &= false;
			}
			disableBackReferencePop.Pop();
			return result;
		}

		protected Boolean NotPredicate(IsMatchPredicate predicate)
		{
			Boolean result = true;
			disableBackReferencePop.Push(true);
			Int32 savePosition = iterator.Index;
			if (!predicate())
			{
				iterator.Index = savePosition;
				result &= true;
			}
			else
			{
				iterator.Index = savePosition;
				result &= false;
			}
			disableBackReferencePop.Pop();
			return result;
		}


		protected Boolean PrioritizedChoice(IsMatchPredicate left, IsMatchPredicate right)
		{
			Int32 savePosition = iterator.Index;

			sandbox.Push(new Stack<AstNode>());
			sandbox.Peek().Push(new AstNode());

			if (left())
			{
				Stack<AstNode> s = sandbox.Pop();
				if (s.Count >= 1)
				{
					AstNode child = s.Pop();
					if (sandbox.Peek().Count == 0)
					{
						sandbox.Peek().Push(child);
					}
					else
					{
						sandbox.Peek().Peek().Children.AddRange(child.Children);
					}
				}

				return true;
			}

			sandbox.Pop();

			iterator.Index = savePosition;

			sandbox.Push(new Stack<AstNode>());
			sandbox.Peek().Push(new AstNode());

			if (right())
			{
				Stack<AstNode> s = sandbox.Pop();
				if (s.Count >= 1)
				{
					AstNode child = s.Pop();
					if (sandbox.Peek().Count == 0)
					{
						sandbox.Peek().Push(child);
					}
					else
					{
						sandbox.Peek().Peek().Children.AddRange(child.Children);
					}
				}

				return true;
			}

			sandbox.Pop();

			return false;
		}

		protected Boolean Sequence(IsMatchPredicate left, IsMatchPredicate right)
		{
			Int32 savePosition = iterator.Index;
			if (left() && right())
			{
				return true;
			}
			else
			{
				iterator.Index = savePosition;
				return false;
			}
		}


		protected Boolean ZeroOrMore(IsMatchPredicate expression, String grammarExpression)
		{
			Int32 savePosition = iterator.Index;
			while (expression())
			{
				if (savePosition == iterator.Index)
				{
					Exception ex = new InfiniteLoopDetectedException();
					ex.Data.Add("Iterator Index", iterator.Index);
					ex.Data.Add("Grammar Expression", grammarExpression);
					throw ex;
				}
				savePosition = iterator.Index;
			}

			iterator.Index = savePosition;

			return true;
		}

		protected Boolean OneOrMore(IsMatchPredicate expression, String grammarExpression)
		{
			Int32 cnt = 0;
			Int32 savePosition = iterator.Index;
			while (expression())
			{
				if (savePosition == iterator.Index)
				{
					Exception ex = new InfiniteLoopDetectedException();
					ex.Data.Add("Iterator Index", iterator.Index);
					ex.Data.Add("Grammar Expression", grammarExpression);
					throw ex;
				}
				savePosition = iterator.Index;
				cnt++;
			}

			iterator.Index = savePosition;
			return (cnt > 0);
		}

		protected Boolean Optional(IsMatchPredicate expression)
		{
			Int32 savePosition = iterator.Index;
			if (expression())
			{
				savePosition = iterator.Index;
			}
			else
			{
				iterator.Index = savePosition;
			}
			return true;
		}

		protected Boolean LimitingRepetition(IsMatchPredicate expression, Int32? min, Int32? max, String grammarExpression)
		{
			Int32 cnt = 0;
			Int32 savePosition = iterator.Index;
			Boolean result = false;

			if (min != null)
			{
				if (max == null)
				{
					// has a minimum but no max cap
					savePosition = iterator.Index;
					while (expression())
					{
						if (savePosition == iterator.Index)
						{
							Exception ex = new InfiniteLoopDetectedException();
							ex.Data.Add("Iterator Index", iterator.Index);
							ex.Data.Add("Grammar Expression", grammarExpression);
							throw ex;
						}
						cnt++;
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
						throw new ArgumentException("A Max property must be larger than Min when using LimitingRepetition.");
					}

					savePosition = iterator.Index;
					while (expression())
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
			else
			{
				if (max == null)
				{
					throw new ArgumentException("A Min and/or Max must be specified when using LimitingRepetition.");
				}
				else
				{
					// zero or up to a max matches of e.
					savePosition = iterator.Index;
					while (expression())
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

			return result;
		}


		protected Boolean CapturingGroup(IsMatchPredicate expression, String name,
		                                 Boolean reduceBySingleChildNode, Boolean doCreateCustomNode)
		{
			if (doCreateCustomNode && pointerAstNodeFactory == null)
			{
				throw new ArgumentNullException(
					"Second constructor overload is required during instantiation.  Function pointer to AstNodeCreate method requires to be set with this parser implementation.");
			}

			Boolean result = true;
			Int32 savePosition = iterator.Index;

			sandbox.Peek().Push(new AstNode());

			if (expression())
			{
				Byte[] matchedBytes = iterator.Text(savePosition, iterator.Index);
				if (backReferenceLookup.ContainsKey(name))
				{
					backReferenceLookup[name].Push(matchedBytes);
				}
				else
				{
					backReferenceLookup.Add(name, new Stack<Byte[]>());
					backReferenceLookup[name].Push(matchedBytes);
				}


				AstNode node = sandbox.Peek().Pop();
				node.Token = new TokenMatch(name, savePosition, iterator.Index, iterator.Text(savePosition, iterator.Index));


				if (doCreateCustomNode)
				{
					// create a custom astnode
					IAstNodeReplacement nodevisitor = pointerAstNodeFactory.Create(node);
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


				if (sandbox.Peek().Count > 0)
				{
					node.Parent = sandbox.Peek().Peek();
					sandbox.Peek().Peek().Children.Add(node);
				}
				else
				{
					sandbox.Peek().Push(node);
					// don't loose the root node
					// each successful sandbox will have 1 item left in the stack
				}

				savePosition = iterator.Index;
				result &= true;
			}
			else
			{
				sandbox.Peek().Pop();
				iterator.Index = savePosition;
				result &= false;
			}

			return result;
		}


#warning matchText should be of type Byte[] and use String.Compare(str1, str2, StringComparison.Ordinal) for culture-agnostic
		protected Boolean Literal(String matchText, Boolean isCaseSensitive)
		{
			foreach (Char c in matchText)
			{
				if (isCaseSensitive)
				{
					if (iterator.Current() == -1)
						return false;

					if ((Byte) iterator.Current() != c)
					{
						iterator.Next();
						return false;
					}
				}
				else
				{
					if (iterator.Current() == -1)
						return false;

					if (Encoding.ASCII.GetString(new[] {(Byte) iterator.Current()}).ToUpper() != c.ToString().ToUpper())
					{
						iterator.Next();
						return false;
					}
				}

				iterator.Next();
			}

			return true;
		}

		protected Boolean CodePoint(String matchCode, Int32 length)
		{
			if (matchCode.StartsWith("#x"))
			{
				//hexadecimal evaluation
				String hexinput = matchCode.Substring(2);
				if (hexinput == String.Empty)
					throw new ArgumentException("Hex value specified is empty.");
				if (hexinput.Length%2 != 0)
					hexinput = "0" + hexinput; // on incomplete byte boundaries shift right

				int total_compare_bytes = hexinput.Length/2;

				for (int i = 0; i < total_compare_bytes; i++)
				{
					if (iterator.Current() == -1)
						return false;

					var compareWith = (Byte) iterator.Current();

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
						var tmp = (Byte) hexinput[i*2 + j];
						if (tmp >= (Byte) '0' && tmp <= (Byte) '9')
						{
							tmp = (Byte) ((tmp - (Byte) '0') << nibbleOffset);
						}
						else if (tmp >= (Byte) 'a' && tmp <= (Byte) 'f')
						{
							tmp = (Byte) ((tmp - (Byte) 'A' + 10) << nibbleOffset);
						}
						else if (tmp >= (Byte) 'A' && tmp <= (Byte) 'F')
						{
							tmp = (Byte) ((tmp - (Byte) 'A' + 10) << nibbleOffset);
						}
						else if (tmp == (Byte) 'x' || tmp == (Byte) 'X')
						{
							// don't care ignore nibble comparison
						}
						else
						{
							throw new ArgumentException("Hex value specified contains invalid characters.");
						}

						if (!(tmp == (Byte) 'x' || tmp == (Byte) 'X'))
						{
							if (
								((compareWith & (1 << (nibbleOffset + 3))) >> (nibbleOffset + 3)) !=
								((tmp & (1 << (nibbleOffset + 3))) >> (nibbleOffset + 3)) ||
								((compareWith & (1 << (nibbleOffset + 2))) >> (nibbleOffset + 2)) !=
								((tmp & (1 << (nibbleOffset + 2))) >> (nibbleOffset + 2)) ||
								((compareWith & (1 << (nibbleOffset + 1))) >> (nibbleOffset + 1)) !=
								((tmp & (1 << (nibbleOffset + 1))) >> (nibbleOffset + 1)) ||
								((compareWith & (1 << nibbleOffset)) >> nibbleOffset) != ((tmp & (1 << nibbleOffset)) >> nibbleOffset)
								)
							{
								return false;
							}
						}
					}

					iterator.Next();
				}
			}
			else if (matchCode.StartsWith("#b"))
			{
				// binary evaluation
				String binaryinput = matchCode.Substring(2);
				if (binaryinput == String.Empty)
					throw new ArgumentException("Binary value specified is empty.");
				if (binaryinput.Length%8 != 0)
					binaryinput = "".PadLeft(8 - (binaryinput.Length%8), '0') + binaryinput;
						// on incomplete byte boundaries shift right

				int total_compare_bytes = binaryinput.Length/8;

				for (int i = 0; i < total_compare_bytes; i++)
				{
					if (iterator.Current() == -1)
						return false;

					var compareWith = (Byte) iterator.Current();
					for (Int32 j = 0; j < 8; j++)
					{
						var tmp = (Byte) binaryinput[i*8 + j];
						if (tmp == (Byte) '0')
						{
							tmp = 0x00;
						}
						else if (tmp == (Byte) '1')
						{
							tmp = 0x01;
						}
						else if (tmp == (Byte) 'x' || tmp == (Byte) 'X')
						{
							// don't care ignore nibble comparison
						}
						else
						{
							throw new ArgumentException("Binary value specified contains invalid characters.");
						}

						if (!(tmp == (Byte) 'x' || tmp == (Byte) 'X'))
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
				String decimalinput = matchCode.Substring(1);
				if (decimalinput == String.Empty)
					throw new ArgumentException("Decimal codepoint value specified is empty.");
				if (!Regex.IsMatch(decimalinput, @"^[0-9]+$"))
					throw new ArgumentException("Decimal codepoint value specified contains invalid characters.");
				if (decimalinput.Length > 10) // 4,294,967,295
					throw new ArgumentException(
						"Decimal codepoint value exceeds 4 byte maximum length.  Largest decimal value possible 2^32");

				UInt32 codepoint = UInt32.Parse(decimalinput);
				var c4 = (Byte) (codepoint >> 24);
				var c3 = (Byte) (codepoint >> 16);
				var c2 = (Byte) (codepoint >> 8);
				var c1 = (Byte) (codepoint);
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

		protected Boolean AnyCharacter()
		{
			if (iterator.Index < iterator.Length)
			{
				iterator.Index += 1;
				return true;
			}
			else
			{
				return false;
			}
		}

		protected Boolean CharacterClass(String classExpression, Int32 length)
		{
			if (iterator.Current() == -1)
				return false;

			var builder = new StringBuilder();
			builder.Append((Char) iterator.Current());
			Boolean result = Regex.IsMatch(builder.ToString(), classExpression);
			if (result)
			{
				iterator.Next();
			}
			return result;
		}

		protected Boolean RecursionCall(IsMatchPredicate expression)
		{
			return expression();
		}

		protected Boolean DynamicBackReference(String backReferenceName, Boolean isCaseSensitive)
		{
			var matchBytes = new Byte[1];
			if (disableBackReferencePop.Count <= 0)
				matchBytes = backReferenceLookup[backReferenceName].Pop();
			else
				matchBytes = backReferenceLookup[backReferenceName].Peek();

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
					builder.Append((Char) iterator.Current());
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

		protected Boolean Fatal(String message)
		{
			Exception ex = new ParsingFatalTerminalException(message);
			ex.Data.Add("InputIterator.Index", iterator.Index);
			throw ex;
		}

		protected Boolean Warn(String message)
		{
			Warnings.Add(new Warn(message, iterator.Index));
			return true;
		}
	}
}