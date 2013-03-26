using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPEG.Extensions;

namespace NPEG.GrammarInterpreter
{
	public class LimitingRepetitionVariableLengthExpressionSolver : IAstNodeReplacement
	{
		private readonly IInputIterator _inputIterator;
		private readonly Dictionary<string, byte[]> _variableValues;

		private readonly Stack<Stack<double>> _results = new Stack<Stack<double>>();
		private readonly Stack<Stack<char>> _symbol = new Stack<Stack<char>>();


		public LimitingRepetitionVariableLengthExpressionSolver(IInputIterator inputIterator, Dictionary<string, byte[]> variableValues)
		{
			_inputIterator = inputIterator;
			_variableValues = variableValues;
			_results.Push(new Stack<double>());
		}

		public double Result
		{
			get
			{
				return _results.Peek().Peek();
			}
		}

		public override void VisitEnter(AstNode node)
		{
			switch (node.Token.Name)
			{
				case "Variable":
					var key = node.Children[0].Token.Value(_inputIterator);
					if (!_variableValues.ContainsKey(key))
					{
						throw new ArgumentOutOfRangeException(string.Format("backreference key {0} does not exist.", key));
					}

					uint value = 0;
					foreach (var b in _variableValues[key].ToArray().Reverse())
					{
						value <<= 8;
						value |= (uint)b & 0xFF;
					}
					_results.Peek().Push(value);
					break;
				case "Symbol":
					_symbol.Peek().Push(node.Token.Value(_inputIterator)[0]);
					break;
				case "Digit":
					_results.Peek().Push(double.Parse(node.Token.Value(_inputIterator)));
					break;
				case "Sum":
					_symbol.Push(new Stack<char>());
					_results.Push(new Stack<double>());
					break;
				case "Product":
					_symbol.Push(new Stack<char>());
					_results.Push(new Stack<double>());
					break;
			}
		}

		public override void VisitLeave(AstNode node)
		{
			double left, right, sum_result = 0;
			Stack<double> results;
			switch (node.Token.Name)
			{
				case "Trig":
					if (node.Token.Value(_inputIterator).StartsWith("cos", StringComparison.InvariantCultureIgnoreCase))
					{
						_results.Peek().Push(Math.Cos(_results.Peek().Pop()));
					}
					else if (node.Token.Value(_inputIterator).StartsWith("sin", StringComparison.InvariantCultureIgnoreCase))
					{
						_results.Peek().Push(Math.Sin(_results.Peek().Pop()));
					}
					break;
				case "Sum":
					results = _results.Pop().Reverse();
					var sum_symbolStack = _symbol.Pop().Reverse();
					while (sum_symbolStack.Count != 0)
					{
						var sum_symbolValue = sum_symbolStack.Pop();
						switch (sum_symbolValue)
						{
							case '+':
								left = results.Pop();
								right = results.Pop();
								sum_result = left + right;
								results.Push(sum_result);
								break;
							case '-':
								left = results.Pop();
								right = results.Pop();
								sum_result = left - right;
								results.Push(sum_result);
								break;
							default:
								throw new NotImplementedException();
						}
					}
					_results.Peek().Push(results.Pop());
					break;
				case "Product":
					results = _results.Pop().Reverse();
					var product_symbolStack = _symbol.Pop().Reverse();
					while (product_symbolStack.Count != 0)
					{
						var product_symbolValue = product_symbolStack.Pop();
						switch (product_symbolValue)
						{
							case '*':
								left = results.Pop();
								right = results.Pop();
								sum_result = left * right;
								results.Push(sum_result);
								break;
							case '/':
								left = results.Pop();
								right = results.Pop();
								sum_result = left / right;
								results.Push(sum_result);
								break;
							default:
								throw new NotImplementedException();
						}
					}
					_results.Peek().Push(results.Pop());
					break;
			}

		}
	}
}