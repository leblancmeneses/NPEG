#ifndef ROBUSTHAVEN_TEXT_AST_H
#define ROBUSTHAVEN_TEXT_AST_H

#include <vector>
#include "TokenMatch.h"

namespace RobustHaven
{
	namespace Text 
	{
	  class IAstNodeVisitor;

	  class AstNode 
	  {
	    const bool m_deleteToken;
	    std::vector<AstNode*> m_children;
	    AstNode *m_parent;
	    TokenMatch *m_token;    

	  public:
	    AstNode* getParent(void);

	    void setParent(AstNode *parent);

	    void addChild(AstNode *newChild);

	    std::vector<AstNode*>& getChildren(void);

	    void setToken(TokenMatch *token);

	    TokenMatch* getToken(void);

	  public:
	    void accept(IAstNodeVisitor &visitor);

	  public:
	    /*
	     * routine for recursive deletion of ASTs
	     */
	    static void deleteAST(AstNode *root);

	  public:
	    /*
	     * The parameter deleteToken, if true, tells the AstNode object to delete the token upon destruction.
	     * Default: no deletion
	     */
	    AstNode(const bool deleteToken = false);        

	    /*
	     * The parameter deleteToken, if true, tells the AstNode object to delete the token upon destruction.
	     */
	    AstNode(TokenMatch *token, const bool deleteToken);        

	    virtual ~AstNode(void);
	  };

	  extern inline AstNode* AstNode::getParent() {
	    return m_parent;
	  }

	  extern inline void AstNode::addChild(AstNode *newChild) {
	    m_children.push_back(newChild);
	  }

	  extern inline std::vector<AstNode*>& AstNode::getChildren() {
	    return m_children;
	  }

	  extern inline TokenMatch* AstNode::getToken() {
	    return m_token;
	  }
	}
}
#endif
