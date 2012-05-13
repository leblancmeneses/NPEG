package robusthaven.text;

public abstract class IAstNodeReplacement extends AstNode implements IAstNodeVisitor {
    public IAstNodeReplacement(TokenMatch token) {
	super(token);
    }    
}
