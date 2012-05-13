package robusthaven.text;

public interface IAstNodeVisitor {
    public void visitEnter(AstNode node);
    public void visitLeave(AstNode node);
}
