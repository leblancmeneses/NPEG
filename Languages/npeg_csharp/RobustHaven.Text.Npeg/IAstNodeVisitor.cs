namespace RobustHaven.Text.Npeg
{
	public interface IAstNodeVisitor
	{
		void VisitEnter(AstNode node);
		void VisitExecute(AstNode node);
		void VisitLeave(AstNode node);
	}
}