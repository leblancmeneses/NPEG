namespace NPEG
{
  // visitor
  public abstract class IAstNodeReplacement : AstNode
  {
    public abstract void VisitEnter(AstNode node);
    public abstract void VisitLeave(AstNode node);
  }
}
