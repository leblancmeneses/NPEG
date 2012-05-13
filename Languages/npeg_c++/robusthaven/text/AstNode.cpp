#include "IAstNodeVisitor.h"
#include "AstNode.h"

using std::vector;
using namespace RobustHaven::Text;

void AstNode::setParent(AstNode *parent) {
  m_parent = parent;
}

void AstNode::setToken(TokenMatch *token) {
  m_token = token;
}

AstNode::AstNode(const bool deleteToken) : m_deleteToken(deleteToken) {
  m_token = NULL;
}

AstNode::AstNode(TokenMatch *token, const bool deleteToken) : m_deleteToken(deleteToken) {
  setToken(token);
}

AstNode::~AstNode() {
  if (m_deleteToken && m_token) {
    delete m_token;
  }
}

void AstNode::deleteAST(AstNode *root) {
  vector<AstNode*>::iterator cit;

  for (cit = root->getChildren().begin(); cit < root->getChildren().end(); cit++) {
    deleteAST(*cit);
  }

  delete root;
} 

void AstNode::accept(IAstNodeVisitor &visitor) {
  vector<AstNode*>::iterator cit;

  visitor.visitEnter(*this);

  for (cit = getChildren().begin(); cit < getChildren().end(); cit++) {    
    (*cit)->accept(visitor);
  }  
  visitor.visitLeave(*this);
} 
