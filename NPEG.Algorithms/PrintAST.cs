using System.Diagnostics;

namespace NPEG.Algorithms
{
	public class PrintAST : IAstNodeReplacement
	{
		public override void VisitEnter(AstNode node)
		{
			Debug.WriteLine("VisitEnter: " + node.Token.Name + " " + ((node.Children.Count == 0) ? node.Token.Value : ""));
			Debug.Indent();
		}

		public override void VisitLeave(AstNode node)
		{
			Debug.Unindent();
			Debug.WriteLine("VisitLeave: " + node.Token.Name);
		}
	}
}