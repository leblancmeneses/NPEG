package robusthaven.text;

public class AstNode {
    /* custom vector implementation since existing Java vectors do not provide fast enough access */
    AstNode[] m_children;
    int m_nofChildren, m_childrenCapacity;

    AstNode m_parent;
    TokenMatch m_token;

    public AstNode getParent() {
	return m_parent;
    }

    public void setParent(AstNode parent) {
	m_parent = parent;
    }

    public AstNode[] getChildren() {
	return m_children;
    }

    public int nofChildren() {
	return m_nofChildren;
    }

    public void addChild(AstNode newChild) {	
	if (m_nofChildren + 1 >= m_childrenCapacity) {
	    AstNode[] tmpChildren;

	    m_childrenCapacity *= 2;
	    tmpChildren = m_children;
	    m_children = new AstNode[m_childrenCapacity];
	    System.arraycopy(tmpChildren, 0, m_children, 0, m_nofChildren);
	}
	m_children[m_nofChildren] = newChild;
	m_nofChildren += 1;
    }

    public TokenMatch getToken() {
	return m_token;
    }

    public void setToken(TokenMatch token) {
	m_token = token;
    }

    public void accept(IAstNodeVisitor visitor) {
	visitor.visitEnter(this);

	if (nofChildren() > 0) {
	    AstNode[] children;
	    int i;
		    
	    children = getChildren();
	    for (i = 0; i < nofChildren(); i++) {    
		children[i].accept(visitor);
	    }  
	}
	visitor.visitLeave(this);
    }

    public AstNode(TokenMatch token) {
	m_token = token;
	m_childrenCapacity = 4;
	m_nofChildren = 0;
	m_children = new AstNode[m_childrenCapacity];
    }
}