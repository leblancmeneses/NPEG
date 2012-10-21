namespace RobustHaven.Text.Npeg
{
	public interface IAstNodeFactory
	{
		IAstNodeReplacement Create(AstNode original);
	}
}