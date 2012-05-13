#ifndef ROBUSTHAVEN_TEXT_IASTNODEVISITOR_H
#define ROBUSTHAVEN_TEXT_IASTNODEVISITOR_H

#include "AstNode.h"


namespace RobustHaven
{
  namespace Text 
  {
    class IAstNodeVisitor {
    public:
      virtual void visitEnter(AstNode &node) = 0;
      virtual void visitLeave(AstNode &node) = 0;
    };
  }
}

#endif
