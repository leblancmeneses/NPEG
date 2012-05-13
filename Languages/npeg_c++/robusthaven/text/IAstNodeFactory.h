#ifndef ROBUSTHAVEN_TEXT_ASTNODEFACTORY_H
#define ROBUSTHAVEN_TEXT_ASTNODEFACTORY_H

#include "IAstNodeReplacement.h"

namespace RobustHaven
{
  namespace Text 
  {
    class IAstNodeFactory {
    public:
      virtual IAstNodeReplacement* create(AstNode *oldNode) = 0;
    };
  }
}

#endif
