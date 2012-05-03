namespace NPEG
{
	public interface IAstNodeFactory
	{
		IAstNodeReplacement Create(AstNode original);
	}
}