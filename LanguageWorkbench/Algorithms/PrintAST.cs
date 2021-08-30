using NPEG;
using NPEG.Extensions;

namespace LanguageWorkbench.Algorithms
{
	public class PrintAST : IAstNodeReplacement
	{
		private readonly IInputIterator _inputIterator;

		private readonly IndentedStringBuilder _sb = new IndentedStringBuilder();

		public PrintAST(IInputIterator inputIterator)
		{
			_inputIterator = inputIterator;
		}

		public override void VisitEnter(AstNode node)
		{
			_sb.AppendLine("VisitEnter: " + node.Token.Name + " " + ((node.Children.Count == 0) ? node.Token.ValueAsString(_inputIterator) : ""));
			_sb.IncreaseIndent();
		}

		public override void VisitLeave(AstNode node)
		{
			_sb.DecreaseIndent();
			_sb.AppendLine("VisitLeave: " + node.Token.Name);
		}

		public override string ToString()
		{
			return _sb.ToString();
		}
	}
}