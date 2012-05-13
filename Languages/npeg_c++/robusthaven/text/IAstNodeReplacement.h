#ifndef ROBUSTHAVEN_TEXT_IASTNODEREPLACEMENT_H
#define ROBUSTHAVEN_TEXT_IASTNODEREPLACEMENT_H

#include "IAstNodeVisitor.h"
#include "AstNode.h"


namespace RobustHaven
{
  namespace Text 
  {
    class IAstNodeReplacement : public AstNode, public IAstNodeVisitor {      
    public:
      IAstNodeReplacement(TokenMatch *token, const bool deleteToken) : AstNode(token, deleteToken) {}

      virtual ~IAstNodeReplacement(void) {}
    };
  }
}

#endif
